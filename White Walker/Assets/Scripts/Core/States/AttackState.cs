using System;
using UnityEngine;
using static AttackBase;

public class AttackState : BaseState
{
    private float attackTimer;
    private int eventIndex;

    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Attack State");

        fighter.ConsumeAttackInput();

        attackTimer = 0f;
        eventIndex = 0;

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        if (fighter.currentAttack == null)
            return;

        attackTimer += Time.deltaTime;

        var atk = fighter.currentAttack;

        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();
        double normalizedTime = playableSystem.GetAttackNormalizedTime();

        if (attackTimer >= atk.TotalDuration)
        {
            fighter.GetComponent<PlayableAnimationFighter>().StopAttack(.15f);
            fighter.ChangeState(fighter.IdleState);
        }
 
        // PROCESAR EVENTOS
        while (eventIndex < atk.events.Count && attackTimer >= atk.events[eventIndex].time)
        {
            ProcessEvent(atk.events[eventIndex]);
            eventIndex++;
        }

        // BLOQUEAR MOVIMIENTO
        fighter.MoveEntity(Vector3.zero, 0);

        // VENTANA DE COMBO
        if (attackTimer >= atk.comboWindowStart)
        {
            if (fighter.GetAttackInput())
            {
                AdvanceCombo();
                return;
            }
        }

        // FIN DEL ATAQUE
        if (attackTimer >= atk.TotalDuration)
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    private void ProcessEvent(AttackEvent e)
    {
        switch (e.type)
        {
            case AttackEventType.OpenHitbox:
                fighter.OpenHitbox(e.hitboxIndex);
                break;

            case AttackEventType.CloseHitbox:
                fighter.CloseHitbox(e.hitboxIndex);
                break;
        }
    }

    public void AdvanceCombo()
    {
        fighter.comboIndex++;

        if (fighter.comboIndex >= fighter.activeCombo.attacks.Count)
            fighter.comboIndex = 0;

        fighter.currentAttack = fighter.activeCombo.attacks[fighter.comboIndex];
        fighter.ConsumeAttackInput();

        EnterState();
    }

    private void PlayAttackAnimation()
    {
        var attack = fighter.currentAttack;
        if (attack == null) { fighter.ChangeState(fighter.IdleState); return; }

        var PlayClip = fighter.GetComponent<PlayableAnimationFighter>();
        PlayClip.PlayClip(attack.animation, attack.blendTime);

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