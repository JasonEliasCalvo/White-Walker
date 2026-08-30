using UnityEngine;

public class PlayerFighter : FighterEntity
{
    [Header("Player")]
    [SerializeField] private PlayerInputSource playerInput;

    [Header("Dash")]
    [SerializeField]
    private float dashCooldown = 0.8f;
    private float dashCooldownTimer;

    [Header("Attack Buffer")]
    [SerializeField]
    private float bufferWindow = 1f;

    private bool attackBuffer;
    private float bufferTimer;

    // Estado único
    private float dashCooldownCounter;

    protected override void Awake()
    {
        base.Awake();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInputSource>();

        if (playerInput == null)
        {
            Debug.LogError(
                $"{name}: PlayerFighter necesita PlayerInputSource.",
                this
            );

            return;
        }

        movement.SetInputSource( playerInput );
    }

    protected override void Start()
    {
        base.Start();

        if (GameManager.instance != null)
        {
            GameManager.instance.InitialGameStart += HandleGameStart;
            GameManager.instance.InitialGameEnd += HandleGameEnd;
        }
    }

    protected override void Update()
    {
        RegisterAttackInput();

        base.Update();

        UpdateAttackBuffer();
        HandlePlayerActions();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    public override bool HasAttackInput()
    {
        return attackBuffer;
    }

    public override void ConsumeAttackInput()
    {
        attackBuffer = false;
        bufferTimer = 0f;
    }

    private void RegisterAttackInput()
    {
        if (playerInput != null && playerInput.ConsumeAttackPressed())
        {
            attackBuffer = true;
            bufferTimer = bufferWindow;
        }
    }

    private void UpdateAttackBuffer()
    {
        if (!attackBuffer)
            return;

        bufferTimer -= Time.deltaTime;

        if (bufferTimer <= 0f)
        {
            attackBuffer = false;
            bufferTimer = 0f;
        }
    }

    private void HandlePlayerActions()
    {
        if (currentState != null && !currentState.CanBeInterrupted)
        {
            return;
        }

        if (playerInput == null)
            return;

        if (movement.IsGrounded && playerInput.ConsumeJumpPressed())
        {
            Debug.Log("ConsumeJumpPressed ");
            Movement.StartDisplacement( moveSet.jumpDisplacement,Vector3.zero);
            ChangeState(AirborneState);
            return;
        }

        if (dashCooldownTimer <= 0f && movement.IsGrounded && playerInput.ConsumeDashPressed())
        {
            ChangeState(dodgeState);
            return;
        }

        if (playerInput.ConsumeInteractPressed())
        {
            // InteractState posteriormente.
        }
    }

    public void StartDashCooldown()
    {
        dashCooldownCounter = dashCooldown;
    }

    private void HandleGameStart()
    {
        movement.SetHorizontalMovementEnabled(true);
    }

    private void HandleGameEnd()
    {
        movement.StopHorizontalMovement();
        movement.SetHorizontalMovementEnabled(false);
    }
}