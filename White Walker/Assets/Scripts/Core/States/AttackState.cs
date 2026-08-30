using System;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
       Debug.Log("Entered Attack State");

        fighter.Movement.StopHorizontalMovement();
        fighter.Movement.SetHorizontalMovementEnabled(false);

        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {   
        if (fighter.currentAttack == null)
            return;

        AnimatorStateInfo info = fighter.animator.GetCurrentAnimatorStateInfo(0);

        // --- VENTANA DE CANCELACIÓN ---
        if (info.normalizedTime > 0.8f)
        {
            if (fighter.HasAttackInput())
            {
                Debug.Log("Input detected for next attack in combo!");
                AdvanceCombo();
            }
        }

        if (info.normalizedTime >= 0.90f)
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public void AdvanceCombo()
    {
        fighter.comboIndex++;
        if (fighter.comboIndex >= fighter.moveSet.attacks.Count)
        {
            fighter.comboIndex = 0;
        }

        fighter.currentAttack = fighter.moveSet.attacks[fighter.comboIndex];
        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        AttackData attack = fighter.currentAttack;

        if (attack == null)
        {
            fighter.ChangeState(fighter.IdleState);
            return;
        }

        fighter.animator.CrossFade(
            attack.animationStateName,
            0.05f
        );

        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
        fighter.AnimEvent_CloseHitbox(4);
    }

    public void OnAttackEnd()
    {
        fighter.ChangeState(fighter.IdleState);
    }

    public override void ExitState()
    {
        fighter.Movement.SetHorizontalMovementEnabled(true);

        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
        fighter.AnimEvent_CloseHitbox(4);
    }

    public override void FixedUpdateState()
    {

    }
}