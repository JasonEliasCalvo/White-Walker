using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(FighterEntity fighter) : base(fighter) {}

    public override void EnterState()
    {
        Debug.Log( $"{fighter.name} → LOCOMOTION STATE");
        fighter.Movement.SetHorizontalMovementEnabled(true);
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }
}
