using UnityEngine;

public interface IFighterInput
{
    bool AttackPressed();
    bool DodgePressed();
    Vector2 MoveInput();
    void ConsumeInput();
}

// Requerimos estos componentes obligatoriamente
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class FighterEntity : MonoBehaviour, IDamageable
{

    [Header("Core Components")]
    public Animator animator;
    protected HealthComponent health;
    public CharacterController controller;

    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.12f;

    [Header("Frame Logic")]
    private const float TICK_RATE = 1f / 60f; // 60 FPS Lógicos (0.01666... seg)
    private float tickTimer = 0f;

    // Físicas
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float verticalVelocity;
    protected float rotationVelocity;

    // Estado Actual
    protected BaseState currentState;

    // --- ESTADOS (Instancias) ---
    public IdleState IdleState;
    public WalkState WalkState;
    public DodgeState dodgeState;
    public AerialState aerialState;
    public AttackState AttackState;
    public HurtState HurtState;
    public DeathState DeathState;

    // --- COMBAT REFERENCES ---
    [Header("Combat System")]
    public CombatHitbox rightHandBox; // 0
    public CombatHitbox leftHandBox; // 1
    public CombatHitbox rightFootBox; // 2
    public CombatHitbox leftFootBox; // 3
    public CombatHitbox weaponBox; // 4

    [Header("Combo Settings")]
    public ComboSequence activeCombo;
    [HideInInspector] public int comboIndex = 0;

    public virtual void ConsumeAttackInput() { }

    public bool IsStunned { get; private set; }
    public bool IsVulnerable { get; private set; }
    public bool IsInvulnerable { get; set; }

    // El ataque que se está ejecutando
    public AttackData currentAttack;
    public IFighterInput InputHandler { get; private set; }

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();
        InputHandler = GetComponent<IFighterInput>();

        // Inicializamos estados pasando "this" (la entidad)
        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        aerialState = new AerialState(this);

        AttackState = new AttackState(this);
        HurtState = new HurtState(this);
        DeathState = new DeathState(this);

        // SUSCRIPCIONES IMPORTANTES
        health.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        ChangeState(IdleState);
    }

    protected virtual void Update()
    {
        if (currentState == DeathState) return;

        tickTimer += Time.deltaTime;
        while (tickTimer >= TICK_RATE)
        {
            TickLogic();
            tickTimer -= TICK_RATE;
        }

        ApplyGravity();

        Vector3 finalMove = velocity + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void TickLogic()
    {
        currentState?.UpdateState();
    }

    // --- SISTEMA DE DAÑO ---
    public virtual void TakeDamage(float amount, float hitStun)
    {
        if (IsInvulnerable || currentState == DeathState) return;

        health.ApplyDamage(amount);
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {health.CurrentHealth}");

        if (health.CurrentHealth > 0)
        {
            if (currentState == HurtState)
            {
                // Si ya estamos golpeados, llamamos al método especial de refresco
                HurtState.RefreshHit(hitStun);
            }
            else
            {
                // Si es el primer golpe, entramos al estado normalmente
                HurtState.stunDuration = hitStun;
                ChangeState(HurtState);
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

    // --- FÍSICAS COMPARTIDAS ---
    public void MoveEntity(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            // Rotación
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Velocidad
            velocity = direction * speed;
        }
        else
        {
            velocity = Vector3.zero;
        }

        // Animación
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
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;
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

    // --- HITBOX MANAGEMENT ---
    public void OpenHitbox(int limbIndex, float dmg, float stun, float knock)
    {
        if (currentAttack == null) return;

        switch (limbIndex)
        {
            case 0: rightHandBox?.EnableHitbox(dmg, stun, knock); break;
            case 1: leftHandBox?.EnableHitbox(dmg, stun, knock); break;
            case 2: rightFootBox?.EnableHitbox(dmg, stun, knock); break;
            case 3: leftFootBox?.EnableHitbox(dmg, stun, knock); break;
            case 4: weaponBox?.EnableHitbox(dmg, stun, knock); break;
        }
    }

    public void CloseHitbox(int limbIndex)
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

    public void CloseAllHitBox()
    {
        rightFootBox?.DisableHitbox();
        leftFootBox?.DisableHitbox();
        rightHandBox?.DisableHitbox();
        leftHandBox?.DisableHitbox();
        weaponBox?.DisableHitbox();
    }

    // --- MÉTODOS ABSTRACTOS ---
    public abstract Vector3 GetMovementInput();
    public abstract bool GetAttackInput();
}
