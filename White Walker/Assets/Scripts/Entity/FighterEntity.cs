using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterMovement))]
public abstract class FighterEntity : MonoBehaviour, IDamageable
{
    [Header("Core Components")]
    public Animator animator;
    public HealthComponent health;
    private CharacterMovement movement;
    public AudioSource audioSource;
    protected ActionResolver actionResolver;
    protected FighterAnimator fighterAnimator;
    public UnityEvent onDeathEnd;

    [Header("Input")]
    [SerializeField] protected CharacterInputSource inputSource;

    [Header("State")]
    protected BaseState currentState;

    public LocomotionState LocomotionState { get; private set; }
    public ActionState ActionState { get; private set; }

    public HitState HitState;
    public DeathState DeathState;

    [Header("Combat System")]
    public CombatHitbox rightHandBox; // 0
    public CombatHitbox leftHandBox; // 1
    public CombatHitbox rightFootBox; // 2
    public CombatHitbox leftFootBox; // 3
    public CombatHitbox weaponBox; // 4

    [Header("Combo Settings")]
    public MoveSet moveSet;
    public ActionData currentAtion;
    public MoveSet defaultMoveSet;
    [HideInInspector] public int comboIndex = 0;

    public bool IsStunned { get; private set; }
    public bool IsInvulnerable { get; set; }

    public Vector3 MovementInput =>
    inputSource != null
        ? inputSource.GetMovementDirection()
        : Vector3.zero;

    public CharacterMovement Movement => movement;
    public CharacterInputSource InputSource => inputSource;
    public ActionResolver ActionResolver => actionResolver;
    public BaseState CurrentState { get => currentState; set => currentState = value; }
    public FighterAnimator FighterAnimator => fighterAnimator;
    protected virtual void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        health = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        inputSource = GetComponent<CharacterInputSource>();
        actionResolver = new ActionResolver();
        fighterAnimator = GetComponent<FighterAnimator>();

        if (movement != null && inputSource != null)
            movement.SetInputSource(inputSource);

        LocomotionState = new LocomotionState(this);
        ActionState = new ActionState(this);
        HitState = new HitState(this);
        DeathState = new DeathState(this);

        health.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        if (moveSet == null)
            moveSet = defaultMoveSet;

        ChangeState(LocomotionState);
    }

    protected virtual void Update()
    {
        currentState?.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    // --- DAMAGE ---
    public virtual void TakeDamage(float amount, float hitStun)
    {
        if (IsInvulnerable || currentState == DeathState) return;

        health.ApplyDamage(amount);

        if (health.CurrentHealth > 0)
        {
            if (currentState == HitState)
            {
                HitState.RefreshHit(hitStun);
            }
            else
            {
                HitState.stunDuration = hitStun;
                ChangeState(HitState);
            }
        }
    }

    public void Heal(float amount)
    {
        health.Heal(amount);
    }

    private void HandleDeath()
    {
        if (currentState == DeathState)
            return;

        ChangeState(DeathState);
    }

    public void InstantDeath()
    {
        if (currentState == DeathState)
            return;

        health.ApplyDamage(health.CurrentHealth);
        ChangeState(DeathState);
    }

    // --- STATES ---
    public void ChangeState(BaseState newState)
    {
        if (newState == null)
            return;

        if (currentState == newState) return;

        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    protected virtual void ExecuteAction(ActionData action)
    {
        if (action == null)
            return;

        if (ActionState == null)
        {
            Debug.LogError(
                $"{name}: ActionState no está asignado."
            );

            return;
        }

        ActionState.SetAction(action);
        ChangeState(ActionState);
    }

    public void ResetState()
    {
        currentState?.ExitState();
        currentState?.EnterState();
    }

    public void ResetCombo()
    {
        comboIndex = 0;
    }

    public virtual void SetMoveSet(MoveSet newMoveSet)
    {
        if (newMoveSet == null)
        {
            Debug.LogWarning($"{gameObject.name}: Combo nulo.");
            return;
        }

        moveSet = newMoveSet;
        comboIndex = 0;
        currentAtion = null;

        Debug.Log($"{gameObject.name} cambió a combo: {newMoveSet.name}");
    }

    // --- HITBOX MANAGEMENT ---
    public void AnimEvent_OpenHitbox(int limbIndex)
    {
        if (currentAtion == null) return;

        float dmg = currentAtion.damage;
        float stun = currentAtion.hitStun;
        float knock = currentAtion.knockbackForce;

        CombatHitbox targetBox = GetHitbox(
                   limbIndex
               );

        if (targetBox == null)
        {
            Debug.LogError(
                $"{gameObject.name}: No existe CombatHitbox para limbIndex {limbIndex}."
            );

            return;
        }

        Debug.Log(
            $"<color=cyan>OPEN HITBOX:</color> {gameObject.name} | " +
            $"Ataque: {currentAtion.actionName} | " +
            $"Hitbox: {targetBox.gameObject.name}"
        );

        targetBox?.EnableHitbox(dmg, stun, knock, currentAtion);
    }

    public void AnimEvent_CloseHitbox(int limbIndex)
    {
        CombatHitbox targetBox =
                    GetHitbox(limbIndex);

        targetBox?.DisableHitbox();
    }

    private CombatHitbox GetHitbox(
       int limbIndex)
    {
        return limbIndex switch
        {
            0 => rightHandBox,
            1 => leftHandBox,
            2 => rightFootBox,
            3 => leftFootBox,
            4 => weaponBox,
            _ => null
        };
    }

    public void AnimEvent_PlaySwingSound()
    {
        if (currentAtion != null && currentAtion.swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentAtion.swingSound);
        }
    }

    public void AnimEvent_SpawnAttackParticle(int limbIndex)
    {
        if (currentAtion == null || currentAtion.swingParticlePrefab == null) return;

        Transform targetTransform = transform;

        switch (limbIndex)
        {
            case 0: if (rightHandBox != null) targetTransform = rightHandBox.transform; break;
            case 1: if (leftHandBox != null) targetTransform = leftHandBox.transform; break;
            case 2: if (rightFootBox != null) targetTransform = rightFootBox.transform; break;
            case 3: if (leftFootBox != null) targetTransform = leftFootBox.transform; break;
            case 4: if (weaponBox != null) targetTransform = weaponBox.transform; break;
        }

        GameObject vfx = Instantiate(currentAtion.swingParticlePrefab, targetTransform.position, targetTransform.rotation);
        Destroy(vfx, 2f);
    }

    public void AnimEvent_PlayAudioDirect(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
