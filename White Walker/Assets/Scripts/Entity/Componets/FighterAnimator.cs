using UnityEngine;

public class FighterAnimator : MonoBehaviour
{
    private FighterEntity fighter;
    private Animator animator;
    private CharacterMovement movement;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private static readonly int IsActionHash = Animator.StringToHash("IsAction");

    private void Awake()
    {
        fighter = GetComponent<FighterEntity>();
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();

        if (fighter == null)
            Debug.LogError($"{name}: FighterAnimator necesita FighterEntity.", this);

        if (animator == null)
            Debug.LogError($"{name}: FighterAnimator necesita Animator.", this);

        if (movement == null)
            Debug.LogError($"{name}: FighterAnimator necesita CharacterMovement.", this);
    }

    private void Update()
    {
        if (animator == null || movement == null)
            return;

        UpdateLocomotionParameters();
    }

    private void UpdateLocomotionParameters()
    {
        Vector3 input = fighter.MovementInput;
        float speed = input.magnitude;
        bool grounded = movement.IsGrounded;

        float verticalVelocity =
            movement.VerticalVelocity;

        bool falling = !grounded && verticalVelocity < -2f;

        animator.SetFloat(
            SpeedHash,
            speed
        );

        animator.SetBool(
            IsGroundedHash,
            grounded
        );

        animator.SetFloat(
            VerticalVelocityHash,
            verticalVelocity
        );

        animator.SetBool(
            IsFallingHash,
            falling
        );
    }
    public void SetActionPlaying(bool value)
    {
        animator.SetBool(
            IsActionHash,
            value
        );
    }
}