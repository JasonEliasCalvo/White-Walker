using UnityEngine;

public class PlayerFighter : FighterEntity
{
    [Header("Player")]
    [SerializeField] private PlayerInputSource playerInput;

    [Header("Input Buffer")]
    [SerializeField] private float inputBufferDuration = 0.25f;

    private InputBuffer inputBuffer;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 0.8f;
    private float dashCooldownTimer;

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

        Movement.SetInputSource( playerInput );
        inputBuffer = new InputBuffer(inputBufferDuration);
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
        ReadInputCommands();

        inputBuffer.Update();

        base.Update();

        HandlePlayerActions();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }

    private void ReadInputCommands()
    {
        if (playerInput == null)
            return;

        while (playerInput.TryConsumeCommand(out InputCommand command))
        {
            inputBuffer.Add(command);

            Debug.Log(
                $"INPUT BUFFER → {command.type} | " +
                $"Direction: {command.direction}"
            );
        }
    }

    private void HandlePlayerActions()
    {
        if (currentState != null && !currentState.CanBeInterrupted)
            return;

        if (inputBuffer.TryPeek(out InputCommand command))
        {
            ActionContext context = new ActionContext(
                Movement.IsGrounded,
                playerInput.RawMovementInput.magnitude > 0.5f,
                null
            );

            ActionData action =
                actionResolver.Resolve(
                    command,
                    context,
                    moveSet
                );

            if (action != null)
            {
                Debug.Log(
                    $"ACTION RESOLVED → {action.actionName}"
                );

                inputBuffer.TryConsume(out command);

                ExecuteAction(action);
            }
        }

        // Interact posteriormente.
    }

    public bool TryConsumeInputCommand(out InputCommand command)
    {
        return inputBuffer.TryConsume(out command);
    }

    public bool TryPeekInputCommand(out InputCommand command)
    {
        return inputBuffer.TryPeek(out command);
    }

    public void ClearInputBuffer()
    {
        inputBuffer.Clear();
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = dashCooldown;
    }
    private void HandleGameStart()
    {
        Movement.SetHorizontalMovementEnabled(true);
    }

    private void HandleGameEnd()
    {
        Movement.StopHorizontalMovement();
        Movement.SetHorizontalMovementEnabled(false);
    }
}