using UnityEngine;

public class HitState : BaseState
{
    public float stunDuration = 0.5f;
    private float timer;

    private bool mirrorToggle = false;

    public HitState(FighterEntity fighter) : base(fighter) { }
    public override bool CanBeInterrupted => false;

    public override void EnterState()
    {
        Debug.Log("Entered Hit State");
        PlayHitAnimation();
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public void RefreshHit(float newDuration)
    {
        Debug.Log("Hit Refreshed!");
        stunDuration = newDuration;

        mirrorToggle = !mirrorToggle;

        PlayHitAnimation();
    }

    private void PlayHitAnimation()
    {
        fighter.Movement.SetHorizontalMovementEnabled(false);
        timer = stunDuration;

        fighter.animator.SetBool("MirrorHit", mirrorToggle);
        fighter.animator.Play("Hit", -1, 0f);

        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
    }

    public override void ExitState()
    {
        mirrorToggle = false;
        fighter.Movement.SetHorizontalMovementEnabled(true);
    }

    public override void FixedUpdateState() { }
}