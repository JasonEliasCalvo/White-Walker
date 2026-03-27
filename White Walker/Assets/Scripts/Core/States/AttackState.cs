using System;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FighterEntity fighter) : base(fighter) { }

    private float attackTimer;

    public override void EnterState()
    {
        Debug.Log("Entered Attack State");

        attackTimer = 0f;
        fighter.ConsumeAttackInput();
        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        attackTimer += Time.deltaTime;

        var atk = fighter.currentAttack;
        if (atk == null) return;

        fighter.MoveEntity(Vector3.zero, 0);

        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();
        double normalizedTime = playableSystem.GetAttackNormalizedTime();

        // --- VENTANA DE CANCELACIÓN ---
        if (normalizedTime > 0.6f)
        {
            if (fighter.GetAttackInput())
            {
                Debug.Log("Input detected for next attack in combo!");
                AdvanceCombo();
                return;
            }
        }

        if (normalizedTime >= 0.95f)
        {
            fighter.GetComponent<PlayableAnimationFighter>().StopAttack(.15f);
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

        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();

        // 0.1f es el tiempo de mezcla. God Hand es rápido, así que valores bajos funcionan mejor.
        playableSystem.PlayClip(attack.animation, 0.1f);
        
        fighter.CloseAllHitBox();
    }

    public override void ExitState()
    {
        fighter.CloseAllHitBox();
        fighter.currentAttack = null;
    }

    public override void FixedUpdateState()
    {

    }
}