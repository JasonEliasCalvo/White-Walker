using System;
using UnityEngine;

[Flags]
public enum MovementLockSource
{
    None = 0,
    Action = 1 << 0,
    Reaction = 1 << 1,
    External = 1 << 2
}

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LocomotionData locomotionData;

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private CharacterInputSource inputSource;
    [SerializeField] private CharacterDisplacement displacement;

    [Header("Runtime")]
    [SerializeField] private MovementRuntimeData runtimeData;

    [Header("Debug")]
    [SerializeField] private bool drawDebugGizmos = true;

    private MovementLockSource movementLocks = MovementLockSource.None;

    public LocomotionData LocomotionData => locomotionData;
    public CharacterController Controller => controller;
    public MovementRuntimeData RuntimeData => runtimeData;

    public float VerticalVelocity
    {
        get => runtimeData.verticalVelocity;
        set => runtimeData.verticalVelocity = value;
    }
    public bool IsGrounded =>
        controller != null && controller.isGrounded;

    public bool IsHorizontalMovementLocked =>
        movementLocks != MovementLockSource.None;

    public bool IsHorizontalMovementEnabled =>
        movementLocks == MovementLockSource.None;

    public CharacterDisplacement Displacement => displacement;

    public bool IsDisplacing =>
        displacement != null && displacement.IsActive;

    private void Reset()
    {
        controller = GetComponent<CharacterController>();

        displacement = GetComponent<CharacterDisplacement>();
    }

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (displacement == null)
            displacement = GetComponent<CharacterDisplacement>();

        ValidateSetup();

        runtimeData = new MovementRuntimeData();
    }

    private void Update()
    {
        SimulateMovement(Time.deltaTime);
    }

    private void SimulateMovement(float deltaTime)
    {
        if (controller == null || locomotionData == null)
            return;

        runtimeData.isGrounded = controller.isGrounded;

        Vector3 displacementDelta = displacement != null ?
            displacement.SimulateDisplacement(deltaTime) : Vector3.zero;

        Vector3 inputDirection = GetInputDirection();

        if (displacement != null && displacement.BlocksHorizontalMovement)
            inputDirection = Vector3.zero;

        bool useGravity = displacement == null || displacement.UsesGravity;

        CalculateDesiredVelocity(inputDirection);
        CalculateHorizontalVelocity(deltaTime);
        CalculateVerticalVelocity(deltaTime, useGravity);

        ApplyMovement(deltaTime, displacementDelta);
    }

    private Vector3 GetInputDirection()
    {
        if (!IsHorizontalMovementEnabled)
            return Vector3.zero;

        if (inputSource == null)
            return Vector3.zero;

        Vector3 direction = inputSource.GetMovementDirection();

        if (direction.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        direction.y = 0f;

        return direction.normalized;
    }

    private void CalculateDesiredVelocity(Vector3 direction)
    {
        runtimeData.desiredVelocity =
            direction * locomotionData.walkSpeed;
    }

    private void CalculateHorizontalVelocity(float deltaTime)
    {
        Vector3 currentHorizontalVelocity = runtimeData.velocity;
        currentHorizontalVelocity.y = 0f;

        Vector3 targetVelocity = runtimeData.desiredVelocity;

        float rate;

        if (targetVelocity.sqrMagnitude > 0.001f)
        {
            rate = runtimeData.isGrounded
                ? locomotionData.acceleration
                : locomotionData.airAcceleration;
        }
        else
        {
            rate = runtimeData.isGrounded
                ? locomotionData.deceleration
                : locomotionData.airDeceleration;
        }

        Vector3 newHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetVelocity,
            rate * deltaTime
        );

        runtimeData.velocity = new Vector3(
            newHorizontalVelocity.x,
            runtimeData.velocity.y,
            newHorizontalVelocity.z
        );
    }

    private void CalculateVerticalVelocity(float deltaTime, bool useGravity)
    {
        if (!useGravity)
            return;

        if (controller.isGrounded)
        {
            if (runtimeData.verticalVelocity < 0f)
                runtimeData.verticalVelocity = locomotionData.groundedVerticalVelocity;
        }
        else
        {
            float currentGravity = locomotionData.gravity;

            if (runtimeData.verticalVelocity < 0f)
                currentGravity *= locomotionData.fallGravityMultiplier;

            runtimeData.verticalVelocity += currentGravity * deltaTime;

            runtimeData.verticalVelocity = Mathf.Max(
                runtimeData.verticalVelocity,
                -locomotionData.terminalVelocity
            );
        }
    }

    private void ApplyMovement(float deltaTime, Vector3 displacementDelta)
    {
        Vector3 normalMovement =
            new Vector3(
                runtimeData.velocity.x,
                runtimeData.verticalVelocity,
                runtimeData.velocity.z
            ) * deltaTime;

        Vector3 finalMovement =
            normalMovement +
            displacementDelta;

        if (controller.enabled)
            controller.Move(finalMovement);

        RotateTowardsMovement(deltaTime);
    }

    private void RotateTowardsMovement(float deltaTime)
    {
        Vector3 horizontalVelocity = runtimeData.velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.0001f)
            return;

        float targetAngle =
            Mathf.Atan2(
                horizontalVelocity.x,
                horizontalVelocity.z
            ) * Mathf.Rad2Deg;

        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref runtimeData.rotationVelocity,
            locomotionData.rotationSmoothTime
        );

        transform.rotation =
            Quaternion.Euler(0f, angle, 0f);
    }

    // --- MOVEMENT CONTROL ---
    public void SetMovementLock(MovementLockSource source, bool locked)
    {
        if (locked)
            movementLocks |= source;
        else
            movementLocks &= ~source;

        if (IsHorizontalMovementLocked)
        {
            StopHorizontalMovement();
        }
    }

    public void SetHorizontalMovementEnabled(bool enabled)
    {
        SetMovementLock(MovementLockSource.External,
            !enabled
        );
    }

    public void StopHorizontalMovement()
    {
        runtimeData.velocity = new Vector3(
            0f,
            runtimeData.velocity.y,
            0f
        );

        runtimeData.desiredVelocity =
            Vector3.zero;
    }

    public bool StartDisplacement(DisplacementData data,Vector3 direction){
        if (Displacement == null)
            return false;

        return Displacement.StartDisplacement(
            data,
            direction
        );
    }

    public void StopDisplacement()
    {
        Displacement?.StopDisplacement();
    }

    // --- EXTERNAL SETTERS ---
    public void SetInputSource(CharacterInputSource source)
    {
        inputSource = source;
    }

    public void SetLocomotionData(LocomotionData data)
    {
        locomotionData = data;
    }

    public void SetVerticalVelocity(float velocity)
    {
        runtimeData.verticalVelocity = velocity;
    }

    public void AddVerticalVelocity(float amount)
    {
        runtimeData.verticalVelocity += amount;
    }

    private void ValidateSetup()
    {
        if (controller == null)
        {
            Debug.LogError(
                $"{name}: CharacterMovement necesita un CharacterController.",
                this
            );
        }

        if (locomotionData == null)
        {
            Debug.LogWarning(
                $"{name}: No hay LocomotionData asignado.",
                this
            );
        }

        if (inputSource == null)
        {
            Debug.LogWarning(
                $"{name}: No hay CharacterInputSource asignado.",
                this
            );
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawDebugGizmos)
            return;

        Vector3 origin = transform.position;

        // Input / desired direction
        Gizmos.color = Color.green;

        if (runtimeData.desiredVelocity.sqrMagnitude > 0.01f)
        {
            Gizmos.DrawLine(
                origin,
                origin + runtimeData.desiredVelocity
            );
        }

        // Current velocity
        Gizmos.color = Color.blue;

        if (runtimeData.velocity.sqrMagnitude > 0.01f)
        {
            Gizmos.DrawLine(
                origin,
                origin + runtimeData.velocity
            );
        }
    }
#endif
}

[System.Serializable]
public struct MovementRuntimeData
{
    [Header("Velocity")]
    public Vector3 velocity;
    public float verticalVelocity;

    [Header("Desired Movement")]
    public Vector3 desiredVelocity;

    [Header("Rotation")]
    public float rotationVelocity;

    [Header("State")]
    public bool isGrounded;
}
