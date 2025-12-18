using UnityEngine;
using static CameraManager;

public enum MovementState
{
    Walking,
    Sprinting,
    Dashing,
    Airborne,
    Downed,
    Stunned
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    public Transform orientation;
    private Rigidbody rb;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 7.5f;
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Rotation")]
    public float rotationSmoothTime = 0.12f;
    private float rotationVelocity;

    [Header("Gravity")]
    public float gravity = 7.5f;
    public float groundedForce = -1f;

    [Header("Dash")]
    public bool isDashing;
    public bool canAirDash = true;
    public bool airDashUsed;

    [Header("State")]
    public MovementState movementState = MovementState.Walking;

    [Header("Input Buffer")]
    public float dashBufferTime = 0.15f;
    private float dashBufferTimer;

    [Header("Internal")]
    [HideInInspector] public Vector3 inputRaw;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    public bool HasMovementInput => inputRaw.sqrMagnitude > 0.1f;
    private bool usePhysics = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Update()
    {
        ReadInput();
        StateHandler();
        HandleMovement();
        UpdateDashBuffer();
    }


    // =========================
    // INPUT
    // =========================
    public void ReadInput()
    {
        inputRaw = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );
    }

    public void BufferDashInput()
    {
        dashBufferTimer = dashBufferTime;
    }


    private void UpdateDashBuffer()
    {
        if (dashBufferTimer > 0f)
            dashBufferTimer -= Time.deltaTime;
    }

    public bool ConsumeDashBuffer()
    {
        if (dashBufferTimer > 0f)
        {
            dashBufferTimer = 0f;
            return true;
        }
        return false;
    }
    // =========================
    // STATE MACHINE
    // =========================
    private void StateHandler()
    {
        if (isDashing)
        {
            movementState = MovementState.Dashing;
            return;
        }

        if (!controller.isGrounded)
        {
            if (movementState != MovementState.Dashing)
                movementState = MovementState.Airborne;

            return;
        }

        airDashUsed = false;

        if (Input.GetKey(KeyCode.LeftShift) && HasMovementInput && CanProcessMovement())
        {
            movementState = MovementState.Sprinting;
            return;
        }

        movementState = MovementState.Walking;
    }

    public void ForceState(MovementState newState)
    {
        movementState = newState;
    }

    public void EffectState()
    {
        if (movementState == MovementState.Dashing)
        {
            ApplyGravity();
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            return;
        }

        if (movementState == MovementState.Airborne)
        {
            if (controller.isGrounded)
            {
                movementState = MovementState.Walking;
                airDashUsed = false;
            }
            return;
        }

        if (movementState == MovementState.Sprinting)
        {
            if (!Input.GetKey(KeyCode.LeftShift) || !HasMovementInput)
            {
                movementState = MovementState.Walking;
            }
            return;
        }
    }
    // =========================
    // MOVEMENT
    // =========================

    private void HandleMovement()
    {
        ApplyGravity();

        if (movementState == MovementState.Dashing)
        {
            controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            return;
        }

        if (CanProcessMovement())
            HandleHorizontalMovement();
        else
            horizontalVelocity = Vector3.zero;

        Vector3 finalVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void HandleHorizontalMovement()
    {
        Vector3 moveDir = HasMovementInput ? GetMovementDirection(inputRaw) : Vector3.zero;
        float targetSpeed = GetTargetSpeed();

        if (CameraManager.instance.currentStyle == CameraStyle.Basic || CameraManager.instance.currentStyle == CameraStyle.Topdown)
        {
            Vector3 targetVelocity = moveDir * targetSpeed;
            float accel = HasMovementInput ? acceleration : deceleration;

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                accel * Time.deltaTime
            );

            if (HasMovementInput)
                RotateCharacter(moveDir);
        }
        else if (CameraManager.instance.currentStyle == CameraStyle.Combat)
        {

        }
    }

    private float GetTargetSpeed()
    {
        switch (movementState)
        {
            case MovementState.Sprinting:
                return sprintSpeed;
            default:
                return walkSpeed;
        }
    }

    // =========================
    // GRAVITY & GROUND
    // =========================
    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundedForce;
        }
        else
        {
            if (movementState == MovementState.Airborne)
                verticalVelocity += Physics.gravity.y * gravity * Time.deltaTime;
        }
    }

    // =========================
    // HELPERS
    // =========================
    private Vector3 GetMovementDirection(Vector3 input)
    {
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0;
        right.y = 0;

        return (forward * input.z + right * input.x).normalized;
    }

    private void RotateCharacter(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private bool CanProcessMovement()
    {
        return movementState != MovementState.Stunned &&
               movementState != MovementState.Downed &&
               movementState != MovementState.Dashing;
    }

    // =========================
    // DASH API (usado por PlayerDash)
    // =========================
    public bool CanDash()
    {
        if (controller.isGrounded)
            return true;


        if (canAirDash && !airDashUsed)
        {
            airDashUsed = true;
            return true;
        }


        return false;
    }

    public void ResetHorizontalVelocity()
    {
        horizontalVelocity.x = 0f;
        horizontalVelocity.z = 0f;
    }

    // =========================
    // EXTERNAL CONTROL (FUTURO)
    // =========================
    public void SetStunned(bool value)
    {
        movementState = value ? MovementState.Stunned : MovementState.Walking;
    }

    public void SetDowned(bool value)
    {
        movementState = value ? MovementState.Downed : MovementState.Walking;
    }
}
