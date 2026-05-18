using UnityEngine;

public class AttackState : BaseState
{
    private Vector3 currentAcceleration = Vector3.zero;
    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        base.EnterState(); // currentFrame se hace 0 en BaseState

        if (fighter.currentAction == null)
        {
            fighter.ChangeState(fighter.idleState);
            return;
        }

        Debug.Log($"Entered Attack State: {fighter.currentAction.actionName}");

        // Inicializaciones clave heredadas de Small Fight
        fighter.actionHasHit = false;
        currentAcceleration = Vector3.zero;
        fighter.CloseAllHitBox();

        PlayAttackAnimation();
    }

    public override void UpdateState()
    {
        base.UpdateState(); // Incrementa currentFrame en 1

        var atk = fighter.currentAction;
        if (atk == null) return;

        // 1. CONTROL DE HITBOXES (Lógica flanco + multi-hit nativo)
        ProcessHitboxes(atk);

        // 2. GESTIÓN DE PROYECTILES (De Small Fight)
        ProcessProjectiles(atk);

        // 3. AUDIO Y EFECTOS EN FRAMES ESPECÍFICOS (De Small Fight)
        ProcessAudio(atk);

        // 4. CONTROL DE MOVIMIENTO (Combinación de fuerzas lógicas)
        ProcessMovement(atk);

        // 5. CONTROL DE COMBOS Y CANCELACIONES
        if (ProcessCancels(atk))
            return;

        // 6. CONDICIONES DE SALIDA E INTERRUPCIONES DE SMALL FIGHT
        EvaluateStateTransitions(atk);
    }

    private void ProcessHitboxes(ActionData atk)
    {
        if (atk.hitboxes == null) return;

        foreach (var hitbox in atk.hitboxes)
        {
            // Frame exacto de activación
            if (currentFrame == hitbox.startEndFrames.x)
            {
                // Al iniciar una NUEVA ventana de golpe en un ataque multi-hit, 
                // permitimos que vuelva a registrar un impacto limpio.
                fighter.actionHasHit = false;

                fighter.OpenHitbox(hitbox.hitboxIndex, atk.damage, atk.hitStun, atk.knockbackForce);
                Debug.Log($"Hitbox {hitbox.hitboxIndex} ABIERTA en frame {currentFrame}");
            }

            // Frame exacto de desactivación (Sumamos 1 para que cubra el frame final completo)
            if (currentFrame == hitbox.startEndFrames.y + 1)
            {
                fighter.CloseHitbox(hitbox.hitboxIndex);
                Debug.Log($"Hitbox {hitbox.hitboxIndex} CERRADA en frame {currentFrame}");
            }
        }
    }

    private void ProcessProjectiles(ActionData atk)
    {
        if (atk.projectiles == null) return;

        // Adaptación del bucle foreach de proyectiles de Small Fight
        foreach (var proj in atk.projectiles)
        {
            if (currentFrame == proj.frame)
            {
                // Nota: Asegúrate de mapear este método en tu FighterEntity pasándole tu estructura 3D
                // fighter.SpawnProjectile(proj); 
                Debug.Log($"Proyectil instanciado en el frame: {currentFrame}");
            }
        }
    }

    private void ProcessAudio(ActionData atk)
    {
        // Sistema de sonidos Whiff (cuando el golpe falla al aire) extraído de Small Fight
        if (atk.playWhiffSound && currentFrame == atk.whiffPlayFrame)
        {
            AudioSource source = fighter.GetComponent<AudioSource>();
            if (source != null && atk.customWhiffSound != null)
            {
                source.PlayOneShot(atk.customWhiffSound);
            }
        }
    }

    private void ProcessMovement(ActionData atk)
    {
        if (atk.movements == null) return;

        bool appliedMovement = false;

        foreach (var move in atk.movements)
        {
            if (currentFrame >= move.startEndFrames.x && currentFrame <= move.startEndFrames.y)
            {
                Vector3 worldDir = fighter.transform.TransformDirection(move.movementDirection);
                fighter.MoveEntity(worldDir, move.velocity);
                appliedMovement = true;
                break;
            }
        }

        // Freno de mano si no hay movimientos programados para este frame
        if (!appliedMovement)
        {
            fighter.MoveEntity(Vector3.zero, 0);
        }
    }

    private bool ProcessCancels(ActionData atk)
    {
        if (atk.cancels == null) return false;

        var player = fighter as PlayerFighter;
        if (player == null) return false;

        Input3DData input = player.CurrentInputData;

        if (!input.attackPressed) return false;

        foreach (var cancel in atk.cancels)
        {
            // ¿Estamos dentro de la ventana de frames permitida?
            if (currentFrame >= cancel.startEndFrames.x && currentFrame <= cancel.startEndFrames.y)
            {
                // ¿El comando del jugador ("5a", "6a", etc.) coincide con lo que pide este combo?
                if (input.Code == cancel.inputRequired && cancel.nextAction != null)
                {
                    // Hacemos la transición al siguiente ataque de forma inmediata
                    fighter.currentAction = cancel.nextAction;
                    fighter.ResetState(this);
                    return true;
                }
            }
        }
        return false;
    }

    private void EvaluateStateTransitions(ActionData atk)
    {
        // Si el ataque es aéreo pero toca el suelo y tiene activado 'stopOnLanding'
        if (atk.airOkay && atk.stopOnLanding && fighter.controller.isGrounded && currentFrame > 1)
        {
            fighter.ResetCombo();
            fighter.ChangeState(fighter.idleState); // O a un estado de recuperación de aterrizaje (Landing)
            return;
        }

        // Fin natural de la acción por agotamiento de Frames
        if (currentFrame >= atk.numberOfFrames)
        {
            fighter.ResetCombo();
            fighter.ChangeState(fighter.idleState);
        }
    }

    private void PlayAttackAnimation()
    {
        var attack = fighter.currentAction;
        if (attack == null) { fighter.ChangeState(fighter.idleState); return; }

        var playSystem = fighter.GetComponent<PlayableAnimationFighter>();
        if (playSystem != null)
        {
            playSystem.PlayClip(attack.animation, attack.blendTime);
        }
        else
        {
            // Mecánica de emergencia por si usas strings directo en el Animator
            fighter.animator.Play($"Base Layer.{attack.animationName}", -1, 0f);
        }

        Debug.Log("Combo index: " + fighter.comboIndex);
    }

    public override void ExitState()
    {
        base.ExitState();
        fighter.CloseAllHitBox();
        fighter.actionHasHit = false;
        fighter.currentAction = null;
    }
}