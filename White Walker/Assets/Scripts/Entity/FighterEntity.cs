using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class FighterEntity : MonoBehaviour, IDamageable
{
    [Header("Core Components")]
    public Animator animator;
    protected HealthComponent health;
    public CharacterController controller;
    public AudioSource audioSource;
    public UnityEvent onDeath;

    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.12f;

    // Físicas
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float verticalVelocity;
    protected float rotationVelocity;

    // Estado Actual
    protected BaseState currentState;

    // --- ESTADOS (Instancias) ---
    public IdleState IdleState;
    public WalkState WalkState;
    public AirborneState AirborneState;
    public AttackState AttackState;
    public HitState HitState;
    public DeathState DeathState;

    // --- COMBAT REFERENCES ---
    [Header("Combat System")]
    public CombatHitbox rightHandBox; // 0
    public CombatHitbox leftHandBox; // 1
    public CombatHitbox rightFootBox; // 2
    public CombatHitbox leftFootBox; // 3
    public CombatHitbox weaponBox; // 4

    [Header("Combo Settings")]
    public MoveSet moveSet;
    public MoveSet defaultMoveSet;
    [HideInInspector] public int comboIndex = 0;

    public virtual void ConsumeAttackInput() { }

    public bool IsStunned { get; private set; }
    public bool IsVulnerable { get; private set; }
    public bool IsInvulnerable { get; set; }

    public AttackBase currentAttack;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();

        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        AirborneState = new AirborneState(this);

        AttackState = new AttackState(this);
        HitState = new HitState(this);
        DeathState = new DeathState(this);

        health.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        if (moveSet == null)
            moveSet = defaultMoveSet;

        ChangeState(IdleState);
    }

    protected virtual void Update()
    {
        if (currentState == null) return;

        currentState?.UpdateState();
        ApplyGravity();

        Vector3 finalMove = velocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    // --- SISTEMA DE DAÑO ---
    public virtual void TakeDamage(float amount, float hitStun)
    {
        if (IsInvulnerable || currentState == DeathState) return;

        health.ApplyDamage(amount);

        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {health.CurrentHealth}");

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
        if (currentState == DeathState) return;

        ChangeState(DeathState);
        controller.enabled = false;
    }

    public void InstantDeath()
    {
        if (currentState == DeathState)
            return;

        health.ApplyDamage(health.CurrentHealth);
        ChangeState(DeathState);
    }

    // --- FÍSICAS COMPARTIDAS ---
    public void MoveEntity(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            velocity = direction * speed;
        }
        else
        {
            velocity = Vector3.zero;
        }

        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void RotateEntity(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void ApplyGravity()
    {
        if (controller == null) return;

        if (currentState is AttackState)
        {
            verticalVelocity = 0f;
            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;
    }

    // --- GESTIÓN DE ESTADOS ---
    public void ChangeState(BaseState newState)
    {
        if (currentState == newState) return;
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void ResetState(BaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
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
        currentAttack = null;

        Debug.Log($"{gameObject.name} cambió a combo: {newMoveSet.name}");
    }

    // --- HITBOX MANAGEMENT ---
    public void AnimEvent_OpenHitbox(int limbIndex)
    {
        if (currentAttack == null) return;

        float dmg = currentAttack.damage;
        float stun = currentAttack.hitStun;
        float knock = currentAttack.knockbackForce;

        CombatHitbox targetBox = null;

        switch (limbIndex)
        {
            case 0:
                targetBox = rightHandBox; break;
            case 1:
                targetBox = leftHandBox; break;
            case 2:
                targetBox = rightFootBox; break;
            case 3:
                targetBox = leftFootBox; break;
            case 4:
                targetBox = weaponBox; break;
            default:
                Debug.LogError(
                    $"{gameObject.name}: limbIndex inválido: {limbIndex}"
                );
                return;
        }

        if (targetBox == null)
        {
            Debug.LogError(
                $"{gameObject.name}: No existe CombatHitbox para limbIndex {limbIndex}."
            );

            return;
        }

        Debug.Log(
            $"<color=cyan>OPEN HITBOX:</color> {gameObject.name} | " +
            $"Ataque: {currentAttack.attackName} | " +
            $"Hitbox: {targetBox.gameObject.name}"
        );

        targetBox?.EnableHitbox(dmg, stun, knock, currentAttack);
    }

    public void AnimEvent_CloseHitbox(int limbIndex)
    {
        switch (limbIndex)
        {
            case 0: rightHandBox?.DisableHitbox(); break;
            case 1: leftHandBox?.DisableHitbox(); break;
            case 2: rightFootBox?.DisableHitbox(); break;
            case 3: leftFootBox?.DisableHitbox(); break;
            case 4: weaponBox?.DisableHitbox(); break;
        }
    }

    public void AnimEvent_PlaySwingSound()
    {
        if (currentAttack != null && currentAttack.swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentAttack.swingSound);
        }
    }

    public void AnimEvent_SpawnAttackParticle(int limbIndex)
    {
        if (currentAttack == null || currentAttack.swingParticlePrefab == null) return;

        Transform targetTransform = transform;

        switch (limbIndex)
        {
            case 0: if (rightHandBox != null) targetTransform = rightHandBox.transform; break;
            case 1: if (leftHandBox != null) targetTransform = leftHandBox.transform; break;
            case 2: if (rightFootBox != null) targetTransform = rightFootBox.transform; break;
            case 3: if (leftFootBox != null) targetTransform = leftFootBox.transform; break;
            case 4: if (weaponBox != null) targetTransform = weaponBox.transform; break;
        }

        GameObject vfx = Instantiate(currentAttack.swingParticlePrefab, targetTransform.position, targetTransform.rotation);
        Destroy(vfx, 2f);
    }

    public void AnimEvent_PlayAudioDirect(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    // --- MÉTODOS ABSTRACTOS ---
    public abstract Vector3 GetMovementInput();
    public abstract bool GetAttackInput();
}
