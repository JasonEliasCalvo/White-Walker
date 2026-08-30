using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSource : CharacterInputSource
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private PlayerControls controls;

    private Vector2 rawMovementInput;
    private bool attackPressed;
    private bool dashPressed;
    private bool interactPressed;
    private bool jumpPressed;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMoveCanceled;

        controls.Gameplay.Attack.performed += OnAttackPerformed;
        controls.Gameplay.Dash.performed += OnDashPerformed;
        controls.Gameplay.Jump.performed += OnJumpPerformed;
        controls.Gameplay.Interact.performed += OnInteractPerformed;
    }

    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls?.Disable();
    }

    private void OnDestroy()
    {
        controls.Gameplay.Move.performed -= OnMovePerformed;
        controls.Gameplay.Move.canceled -= OnMoveCanceled;

        controls.Gameplay.Attack.performed -= OnAttackPerformed;
        controls.Gameplay.Dash.performed -= OnDashPerformed;
        controls.Gameplay.Jump.performed -= OnJumpPerformed;
        controls.Gameplay.Interact.performed -= OnInteractPerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        rawMovementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        rawMovementInput = Vector2.zero;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        attackPressed = true;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashPressed = true;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        interactPressed = true;
    }

    public override Vector3 GetMovementDirection()
    {
        if (rawMovementInput.sqrMagnitude < 0.01f)
            return Vector3.zero;

        Vector3 forward;
        Vector3 right;

        if (cameraTransform != null)
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();
        }
        else
        {
            forward = Vector3.forward;
            right = Vector3.right;
        }

        Vector3 movement =
            forward * rawMovementInput.y +
            right * rawMovementInput.x;

        return movement.sqrMagnitude > 1f
            ? movement.normalized
            : movement;
    }

    public override bool ConsumeAttackPressed()
    {
        if (!attackPressed)
            return false;

        attackPressed = false;
        return true;
    }

    public override bool ConsumeDashPressed()
    {
        if (!dashPressed)
            return false;

        dashPressed = false;
        return true;
    }

    public override bool ConsumeJumpPressed()
    {
        if (!jumpPressed)
            return false;

        jumpPressed = false;
        return true;
    }

    public bool ConsumeInteractPressed()
    {
        if (!interactPressed)
            return false;

        interactPressed = false;
        return true;
    }

    public Vector2 RawMovementInput => rawMovementInput;
}