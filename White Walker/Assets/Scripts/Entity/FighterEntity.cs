using UnityEngine;

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
    public IdleState idleState;
    public WalkState walkState;
    public DodgeState dodgeState;
    public AerialState aerialState;
    public AttackState attackState;
    public HurtState hurtState;
    public DeathState deathState;

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

    public bool IsInvulnerable { get; set; }

    public ActionData currentAction;
    public bool actionHasHit { get; set; }

    public int currentFrame => currentState != null ? currentState.currentFrame : 0;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();

        // Inicializamos estados pasando "this" (la entidad)
        idleState = new IdleState(this);
        walkState = new WalkState(this);
        aerialState = new AerialState(this);

        attackState = new AttackState(this);
        hurtState = new HurtState(this);
        deathState = new DeathState(this);

        // SUSCRIPCIONES IMPORTANTES
        health.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        ChangeState(idleState);
    }

    protected virtual void Update()
    {
        if (currentState == deathState) return;

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

    public bool GetAttackInput()
    {
        return true;
    }

    public virtual void TakeDamage(float amount, float hitStun)
    {
        if (IsInvulnerable || currentState == deathState) return;

        health.ApplyDamage(amount);
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {health.CurrentHealth}");

        if (health.CurrentHealth > 0)
        {
            if (currentState == hurtState)
            {
                // Si ya estamos golpeados, llamamos al método especial de refresco
                hurtState.RefreshHit(hitStun);
            }
            else
            {
                // Si es el primer golpe, entramos al estado normalmente
                hurtState.stunDuration = hitStun;
                ChangeState(hurtState);
            }
        }
    }

    public void Heal(float amount)
    {
        health.Heal(amount);
    }

    private void HandleDeath()
    {
        if (currentState == deathState) return;

        ChangeState(deathState);
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
        actionHasHit = false;
    }

    // --- HITBOX MANAGEMENT ---
    public void OpenHitbox(int limbIndex, float dmg, float stun, float knock)
    {
        if (currentAction == null) return;

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

    public abstract Vector3 GetMovementInput();
}
