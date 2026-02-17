using UnityEngine;

public class DashState : BaseState
{
    // Creamos una referencia específica para el Jugador
    private PlayerFighter player;
    private Vector3 dashDirection;

    public DashState(FighterEntity entity) : base(entity)
    {
        player = entity as PlayerFighter;
    }

    public override void EnterState()
    {
        Debug.Log("Entered Dash State");

        if (player == null)
        {
            fighter.ChangeState(fighter.IdleState);
            return;
        }

        // 1: Usamos GetMovementInput()
        Vector3 inputDir = fighter.GetMovementInput();

        // Si no hay input, dasheamos hacia donde mira el personaje
        if (inputDir == Vector3.zero)
            dashDirection -= fighter.transform.forward;
        else
            dashDirection = inputDir.normalized;

        // 2: Accedemos a las variables a través de 'player', no de 'fighter'
        player.dashTimer = player.dashDuration;

        // Iniciamos el cooldown en el player
        player.StartDashCooldown();

        fighter.animator?.SetTrigger("Dash");
    }

    public override void UpdateState()
    {
        if (player == null) return;

        player.dashTimer -= Time.deltaTime;

        //  3: Sobrescribimos la velocidad directamente para un movimiento seco y rápido
        fighter.velocity = dashDirection * player.dashSpeed;

        // Mantenemos la velocidad vertical actual (gravedad) o la ponemos a 0 si quieres dash aéreo recto
        fighter.verticalVelocity = 0f; 

        if (player.dashTimer <= 0f)
        {
            // 4: 'HasMovementInput' ahora es una comprobación manual
            if (fighter.GetMovementInput().sqrMagnitude > 0.05f)
            {
                fighter.ChangeState(fighter.WalkState);
            }
            else
            {
                fighter.ChangeState(fighter.IdleState);
            }
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        // CORRECCIÓN 6: Frenado final
        if (player != null)
        {
            fighter.velocity *= player.dashForceStop;
        }
    }
}