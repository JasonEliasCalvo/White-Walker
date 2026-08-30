using UnityEngine;

public class WalkState : BaseState
{
    public WalkState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Walk State");
    }

    public override void FixedUpdateState()
    {
        fighter.Movement.SetHorizontalMovementEnabled(true);
    }

    public override void UpdateState()
    {
        Vector3 moveDir = fighter.MovementInput;

        fighter.animator.SetFloat("Speed", moveDir.magnitude);

        if (moveDir.sqrMagnitude < 0.05f)
            fighter.ChangeState(fighter.IdleState);

        if (!fighter.Movement.IsGrounded && fighter.Movement.VerticalVelocity <= -4f)
            fighter.ChangeState(fighter.AirborneState);

        if (fighter.HasAttackInput())
        {
            fighter.ResetCombo();

            if (fighter.moveSet != null && fighter.moveSet.attacks.Count > 0)
            {
                fighter.currentAttack = fighter.moveSet.attacks[0];
                fighter.ChangeState(fighter.AttackState);
            }
            return;
        }
    }

    public override void ExitState()
    {

    }
}
