using UnityEngine;

public class AttackState : BaseState
{
    private float stateTimer;
    private bool isFinished;

    public AttackState(FighterEntity fighter) : base(fighter) { }

    public override void EnterState()
    {
        Debug.Log("Entered Attack State");

        isFinished = false;
        stateTimer = 0f;

        // 1. Detener el movimiento en seco (Game Feel importante)
        fighter.velocity = Vector3.zero;

        // 2. Reproducir animación
        // Idealmente usaríamos fighter.currentMove.animation.name
    }

    public override void UpdateState()
    {
        stateTimer += Time.deltaTime;

        // LOGICA DE ROTACIÓN ASISTIDA (Auto-aim estilo God Hand)
        // Si hay un enemigo cerca, rotamos suavemente hacia él durante el inicio del golpe
        // (Esto requeriría un sistema de detección de enemigos cercano, por ahora rotamos hacia el input)

        // Detenemos la física de movimiento (God Hand generalmente te ancla al suelo al pegar)
        fighter.MoveEntity(Vector3.zero, 0);

        // Salir del estado cuando la animación termina
        // Una forma simple es usar un timer basado en la duración del clip o un Animation Event "OnAttackEnd"
        AnimatorStateInfo info = fighter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.9f) // Si la animación llegó al 90%
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public override void ExitState()
    {
        // Seguridad: Cerrar hitboxes por si la animación se canceló antes de tiempo
        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
    }

    public override void FixedUpdateState()
    {

    }
}