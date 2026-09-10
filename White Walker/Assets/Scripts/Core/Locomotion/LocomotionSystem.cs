using UnityEngine;

public enum LocomotionPhase
{
    Grounded,
    Airborne
}

public enum LocomotionSubPhase
{
    Idle,
    Moving,
    Rising,
    Falling,
    Suspended
}

[RequireComponent(typeof(CharacterMovement))]
public class LocomotionSystem : MonoBehaviour
{
    [Header("References")]
    private CharacterMovement movement;
    private FighterAnimator fighterAnimator;
    private ActionSystem actionSystem;

    [Header("Current Locomotion")]
    public LocomotionPhase Phase { get; private set; }
    public LocomotionSubPhase SubPhase { get; private set; }


    [Header("Runtime")]
    public bool IsGrounded { get; private set; }
    public bool IsCoyoteActive => coyoteTimer > 0f;
    public bool IsAirborne { get; private set; }

    public bool IsMoving { get; private set; }
    public bool IsRising { get; private set; }
    public bool IsFalling { get; private set; }

    [SerializeField]
    private float movementThreshold = 0.05f;

    [SerializeField]
    private float verticalThreshold = 0.15f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteDuration = 0.15f;
    private float coyoteTimer;

    [Header("Nombres de Animaciones")]
    [SerializeField] private string idleAnimState = "Idle";
    [SerializeField] private string moveAnimState = "Move";
    [SerializeField] private string fallAnimState = "Fall";

    private void Awake()
    {
        if (movement == null) movement = GetComponent<CharacterMovement>();
        if (fighterAnimator == null) fighterAnimator = GetComponent<FighterAnimator>();
        if (actionSystem == null) actionSystem = GetComponent<ActionSystem>();
    }

    private void Update()
    {
        UpdateGroundedAndCoyote();
        UpdateLocomotionPhases();

        if (actionSystem == null || !actionSystem.IsActive)
        {
            UpdateLocomotionAnimations();
        }
    }

    private void UpdateGroundedAndCoyote()
    {
        IsGrounded = movement.IsGrounded;

        if (IsGrounded)
            coyoteTimer = coyoteDuration;
        else
            coyoteTimer -= Time.deltaTime;
    }

    private void UpdateLocomotionPhases()
    {
        float verticalVel = movement.VerticalVelocity;
        float horizontalSpeed = movement.RuntimeData.desiredVelocity.magnitude;

        if (IsGrounded)
        {
            Phase = LocomotionPhase.Grounded;
            SubPhase = horizontalSpeed > 0.1f ? LocomotionSubPhase.Moving : LocomotionSubPhase.Idle;
        }
        else
        {
            Phase = LocomotionPhase.Airborne;
            SubPhase = verticalVel > 0.1f ? LocomotionSubPhase.Rising : LocomotionSubPhase.Falling;
        }
    }

    private void UpdateLocomotionAnimations()
    {
        if (Phase == LocomotionPhase.Grounded)
        {
            if (SubPhase == LocomotionSubPhase.Moving)
            {
                fighterAnimator.PlayLocomotionState(moveAnimState);
            }
            else
            {
                fighterAnimator.PlayLocomotionState(idleAnimState);
            }
        }
        else if (Phase == LocomotionPhase.Airborne)
        {
            if (SubPhase == LocomotionSubPhase.Falling)
            {
                fighterAnimator.PlayLocomotionState(fallAnimState);
            }
        }
    }
}