using Combat;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        base.EnterState(); // currentFrame se hace 0 en BaseState
        Debug.Log($"Entered Attack State: {fighter.currentAttack.attackName}");

        fighter.ConsumeAttackInput();
        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        base.UpdateState(); // Incrementa currentFrame en 1

        var atk = fighter.currentAttack;
        if (atk == null) return;

        // 1. EJECUTAR ACCIONES CON LIFECYCLE (Damage, HitBox, Movement, etc.)
        foreach (var action in atk.actions)
        {
            action.Tick(fighter, currentFrame);
        }

        // 2. Movimiento fallback
        if (!IsMovementActionActive(atk))
        {
            fighter.MoveEntity(Vector3.zero, 0);
        }

        // 3. Cancels
        if (CheckCancels(atk))
            return; // Si el jugador canceló el ataque, salimos de este Update

        // 4. Fin del ataque
        if (currentFrame >= atk.totalFrames)
        {
            fighter.ResetCombo();
            fighter.ChangeState(fighter.IdleState);
        }
    }

    private bool CheckCancels(AttackData atk)
    {
        foreach (var window in atk.cancelWindows)
        {
            if (currentFrame < window.startFrame || currentFrame > window.endFrame)
                continue;

            switch (window.cancelType)
            {
                case CancelType.Attack:
                    if (fighter.GetAttackInput())
                    {
                        AdvanceCombo();
                        return true;
                    }
                    break;

                case CancelType.Dodge:
                    if (fighter.InputHandler.DodgePressed())
                    {
                        fighter.ChangeState(fighter.dodgeState);
                        return true;
                    }
                    break;

                case CancelType.Any:
                    if (fighter.GetAttackInput())
                    {
                        AdvanceCombo();
                        return true;
                    }
                    if (fighter.InputHandler.DodgePressed())
                    {
                        fighter.ChangeState(fighter.dodgeState);
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    private bool IsMovementActionActive(AttackData atk)
    {
        foreach (var action in atk.actions)
        {
            if (action is MovementAction && currentFrame >= action.startFrame && currentFrame <= action.endFrame)
                return true;
        }
        return false;
    }

    public void AdvanceCombo()
    {
        fighter.comboIndex++;

        if (fighter.comboIndex >= fighter.activeCombo.attacks.Count)
            fighter.comboIndex = 0;

        fighter.currentAttack = fighter.activeCombo.attacks[fighter.comboIndex];

        // Re-entramos al estado para reiniciar los frames y lanzar la nueva animación
        fighter.ResetState(this);
    }

    private void PlayAttackAnimation()
    {
        var attack = fighter.currentAttack;
        if (attack == null) { fighter.ChangeState(fighter.IdleState); return; }

        var playSystem = fighter.GetComponent<PlayableAnimationFighter>();
        playSystem.PlayClip(attack.animation, attack.blendTime);

        fighter.CloseAllHitBox();
        Debug.Log("Combo index: " + fighter.comboIndex);
    }

    public override void ExitState()
    {
        base.ExitState();
        fighter.CloseAllHitBox();
        fighter.currentAttack = null;
    }
}