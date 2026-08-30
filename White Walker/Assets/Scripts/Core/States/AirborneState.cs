using UnityEngine;

public class AirborneState : BaseState
{
    public AirborneState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Airborne State");
        fighter.Movement.SetHorizontalMovementEnabled(true);
        UpdateAirborneAnimation();
    }

    public override void UpdateState()
    {
        UpdateAirborneAnimation();

        if (!fighter.Movement.IsGrounded)
            return;

        HandleLanding();
    }

    private void UpdateAirborneAnimation()
    {
        bool isFalling = fighter.Movement.VerticalVelocity < -2.1f && !fighter.movement.IsGrounded;
        fighter.animator?.SetFloat("VerticalVelocity", fighter.Movement.VerticalVelocity);

        if (!isFalling)
            fighter.animator.CrossFade("Jump", 0.2f);
        else
            fighter.animator.CrossFade("Falling", 0.2f);

        fighter.animator?.SetBool("IsFalling",isFalling);
    }

    private void HandleLanding()
    {
        if (fighter.HasAttackInput())
        {
            fighter.ResetCombo();

            if (fighter.moveSet != null && fighter.moveSet.attacks != null && fighter.moveSet.attacks.Count > 0)
            {
                fighter.currentAttack = fighter.moveSet.attacks[0];
                fighter.ChangeState(fighter.AttackState);
            }
            return;
        }

        Vector3 moveDirection = fighter.MovementInput;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            fighter.ChangeState(fighter.WalkState);
        }
        else
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public override void ExitState()
    {
        fighter.animator?.SetBool("IsFalling", false);
        fighter.animator?.SetFloat("VerticalVelocity", 0f);
    }

    public override void FixedUpdateState()
    {
    }
}
