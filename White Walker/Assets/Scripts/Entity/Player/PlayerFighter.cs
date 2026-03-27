using UnityEngine;

public class PlayerFighter : FighterEntity
{
    [Header("Player Specifics")]
    public Transform cameraTransform;
    private TargetLock targetLock;

    // --- VARIABLES DE DASH (Necesarias para DashState) ---
    [Header("Dash Clips")]
    public AnimationClip forwardDashClip;
    public AnimationClip backflipClip;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;      
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;
    public float dashForceStop = 0.1f;

    [HideInInspector] public float dashTimer;
    [HideInInspector] public float dashCooldownTimer;

    // Estado único
    public DashState DashState;

    // Input 
    private PlayerControls controls;
    private Vector2 rawInput;
    private bool dashPressed;
    private bool attackPressed;
    private bool interactPressed;
    private float dashCooldownCounter;

    private bool attackBuffer;
    private float bufferWindow = 0.2f; // Tiempo antes de terminar el ataque donde aceptamos input
    private float bufferTimer;

    public void ClearBuffer() => attackBuffer = false;

    protected override void Awake()
    {
        base.Awake();

        targetLock = GetComponent<TargetLock>();

        DashState = new DashState(this);
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => rawInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => rawInput = Vector2.zero;
        controls.Gameplay.Attack.performed += ctx => attackPressed = true;
        controls.Gameplay.Dash.performed += ctx => dashPressed = true;
        controls.Gameplay.Interact.performed += ctx => interactPressed = true;
        controls.Gameplay.LookTarget.performed += ctx => targetLock.ToggleLock();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    protected override void Update()
    {
        base.Update();

        HandlePlayerActions();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (CameraManager.instance.currentEnemy == null && CameraManager.instance.currentStyle == CameraManager.CameraStyle.Combat)
        {
            targetLock.CleanLock();
        }
    }

    public override Vector3 GetMovementInput()
    {
        if (rawInput.sqrMagnitude < 0.1f) return Vector3.zero;

        // Declaramos las variables de dirección aquí para usarlas abajo
        Vector3 moveForward;
        Vector3 moveRight;

        // --- MODO COMBATE ---
        if (CameraManager.instance.currentStyle == CameraManager.CameraStyle.Combat && CameraManager.instance.currentEnemy != null)
        {
            // El "adelante" es hacia el enemigo
            moveForward = (CameraManager.instance.currentEnemy.position - transform.position).normalized;
            moveForward.y = 0;

            // El "derecha" se calcula cruzando el arriba con la dirección al enemigo
            moveRight = Vector3.Cross(Vector3.up, moveForward);

            return (moveForward * rawInput.y + moveRight * rawInput.x).normalized;
        }

        // --- MODO EXPLORACIÓN (Cámara Libre) ---
        // El "adelante" y "derecha" vienen de la cámara libre
        moveForward = cameraTransform.forward;
        moveRight = cameraTransform.right;
        moveForward.y = 0;
        moveRight.y = 0;

        return (moveForward.normalized * rawInput.y + moveRight.normalized * rawInput.x).normalized;
    }

    public override bool GetAttackInput()
    {
        if (attackPressed)
        {
            attackPressed = false;
            attackBuffer = true;
            bufferTimer = bufferWindow;
        }

        if (attackBuffer)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0) attackBuffer = false;
        }

        return attackBuffer;
    }

    public override void ConsumeAttackInput()
    {
        attackBuffer = false;
        bufferTimer = 0;
    }

    private void HandlePlayerActions()
    {
        if (currentState != null && !currentState.CanBeInterrupted) return;

        // Caso 1: DASH
        if (dashPressed && dashCooldownTimer <= 0 && controller.isGrounded)
        {
            dashPressed = false;
            ChangeState(DashState);
            return;
        }

        // Caso 2: INTERACTUAR (Tirar cajas, etc)
        if (interactPressed)
        {
            // CheckDistanceToBox()...
            // ChangeState(InteractState);
        }
    }

    public void StartDashCooldown()
    {
        dashCooldownCounter = dashCooldown;
    }
}