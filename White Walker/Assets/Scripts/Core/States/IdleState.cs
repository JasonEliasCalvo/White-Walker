using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(FighterEntity pm) : base(pm) { }

    public override void EnterState()
    {
        Debug.Log("Entered Idle State");

        fighter.Movement.SetHorizontalMovementEnabled(true);
        fighter.animator?.SetFloat("Speed", 0f);
    }

    public override void UpdateState()
    {
        Vector3 moveDir = fighter.MovementInput;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            fighter.ChangeState(fighter.WalkState);
            return;
        }

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

        if (!fighter.Movement.IsGrounded && fighter.Movement.VerticalVelocity <= -4f)
            fighter.ChangeState(fighter.AirborneState);
    }

    public override void FixedUpdateState() 
    {
       
    }

    public override void ExitState()
    {
        
    }
}
