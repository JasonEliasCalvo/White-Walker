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

    // Inputs crudos del Unity Input System
    private PlayerControls controls;
    private Vector2 rawInput;
    private bool attackPressed;
    private bool dodgePressed;
    private bool interactPressed;
    private float dashCooldownCounter;

    // Tu nueva estructura de traducción para combos estilo Small Fight
    public Input3DData CurrentInputData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        targetLock = GetComponent<TargetLock>();
        controls = new PlayerControls();

        // Suscripción a eventos de Input
        controls.Gameplay.Move.performed += ctx => rawInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => rawInput = Vector2.zero;
        controls.Gameplay.Interact.performed += ctx => interactPressed = true;
        controls.Gameplay.Attack.performed += ctx => attackPressed = true;
        controls.Gameplay.Dash.performed += ctx => dodgePressed = true;
        controls.Gameplay.LookTarget.performed += ctx => targetLock.ToggleLock();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    protected override void Update()
    {
        Process3DInput();

        base.Update();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Limpieza al final del frame
        attackPressed = false;
        dodgePressed = false;

        if (CameraManager.instance.currentEnemy == null && CameraManager.instance.currentStyle == CameraManager.CameraStyle.Combat)
        {
            targetLock.CleanLock();
        }
    }

    private void Process3DInput()
    {
        Input3DData data = new Input3DData();

        // Dirección relativa (God Hand style)
        if (rawInput.y > 0.3f) data.direction = 6; // Adelante
        else if (rawInput.y < -0.3f) data.direction = 4; // Atrás
        else data.direction = 5; // Neutral

        data.attackPressed = attackPressed;
        data.dodgePressed = dodgePressed;

        CurrentInputData = data;
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
}