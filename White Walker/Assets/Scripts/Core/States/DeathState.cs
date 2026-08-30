using UnityEngine;

public class DeathState : BaseState
{
    public DeathState(FighterEntity fighter) : base(fighter) { }
    public bool isDeathAnimationFinished;

    public override bool CanBeInterrupted => false;

    public override void EnterState()
    {
        Debug.Log("Entered Death State");
        fighter.Movement.StopHorizontalMovement();
        fighter.Movement.SetHorizontalMovementEnabled(false);
        fighter.Movement.Controller.enabled = false;
        fighter.animator.CrossFade("Death", 0.1f);
    }

    public override void UpdateState()
    {
        if (fighter.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && isDeathAnimationFinished == false)
        {
            Debug.Log("Death animation finished");
            isDeathAnimationFinished = true;
            fighter.onDeathEnd?.Invoke();
        }
    }

    public override void ExitState() { }
    public override void FixedUpdateState() { }
}
