using UnityEngine;

public class DodgeState : BaseState
{
    // Creamos una referencia específica para el Jugador
    private AnimationClip selectedClip;
    private PlayerFighter player;
    private Vector3 dashDirection;

    public DodgeState(FighterEntity entity) : base(entity)
    {
        player = entity as PlayerFighter;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Dash State");

        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();
        Vector3 inputDir = fighter.GetMovementInput();

        if (inputDir == Vector3.zero)
        {
            // BACKFLIP (Esquive neutral/atrás)
            dashDirection = -fighter.transform.forward;
            selectedClip = player.backflipClip;
        }
        else
        {
            dashDirection = inputDir.normalized;
            selectedClip = player.forwardDashClip;
        }
        // 2. DISPARAR INSTANTÁNEO
        // Usamos una transición muy corta (0.05s) para que sea un "snap"
        playableSystem.PlayClip(selectedClip, 0.05f);

        // 3. Lógica de físicas
        player.dashTimer = player.dashDuration;

        // IMPORTANTE: En God Hand el Dash tiene I-Frames (Invencibilidad)
        fighter.IsInvulnerable = true;
    }

    public override void UpdateState()
    {
        // 1. Manejo del Tiempo
        if (player.dashTimer > 0)
            player.dashTimer -= Time.deltaTime;

        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();
        double normalizedTime = playableSystem.GetAttackNormalizedTime();

        if (player.dashTimer > 0)
        {
            fighter.velocity = dashDirection * player.dashSpeed;
        }
        else
        {
            // Frenado suave al terminar el tiempo de dash, pero seguimos en el estado
            fighter.velocity = Vector3.Lerp(fighter.velocity, Vector3.zero, Time.deltaTime * 10f);
            fighter.IsInvulnerable = false;
        }

        fighter.verticalVelocity = 0f;

        // 3. Lógica de Salida
        if (player.dashTimer <= 0f)
        {
            fighter.IsInvulnerable = false;

            if (selectedClip == player.backflipClip)
            {
                if (normalizedTime >= 0.95f)
                {
                    ExitToLocomotion();
                }
            }
            else
            {
                ExitToLocomotion();
            }
        }
    }

    private void ExitToLocomotion()
    {
        var playableSystem = fighter.GetComponent<PlayableAnimationFighter>();
        playableSystem.StopAttack(0.15f); // Volvemos suave al Idle/Walk

        if (fighter.GetMovementInput().sqrMagnitude > 0.05f)
        {
            fighter.ChangeState(fighter.walkState);
        }
        else
        {
            fighter.ChangeState(fighter.idleState);
        }
    }

    public override void ExitState()
    {
        fighter.IsInvulnerable = false;
        fighter.velocity *= player.dashForceStop;
    }
}