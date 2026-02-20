using System;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
       Debug.Log("Entered Attack State");

        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {   
        if (fighter.currentAttack == null)
            return;

        fighter.MoveEntity(Vector3.zero, 0);
        AnimatorStateInfo info = fighter.animator.GetCurrentAnimatorStateInfo(0);

        // --- VENTANA DE CANCELACIÓN ---
        if (info.normalizedTime > 0.6f)
        {
            if (fighter.GetAttackInput())
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
        if (fighter.comboIndex >= fighter.activeCombo.attacks.Count)
        {
            fighter.comboIndex = 0;
        }

        fighter.currentAttack = fighter.activeCombo.attacks[fighter.comboIndex];
        fighter.ConsumeAttackInput();

        PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        AttackBase attack = fighter.currentAttack;
        if (attack == null) { fighter.ChangeState(fighter.IdleState); return; }

        // Usando el componente de arriba
        var player = fighter.GetComponent<PlayableAnimationFighter>();
        player.PlayClip(attack.animation, 0.1f);

        // Reset de Hitboxes por seguridad
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