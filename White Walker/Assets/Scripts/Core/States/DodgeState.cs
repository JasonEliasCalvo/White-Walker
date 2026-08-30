using UnityEngine;

public class DodgeState : BaseState
{
    private Vector3 dodgeDirection;

    public DodgeState(FighterEntity pm) : base(pm) { }

    public override void EnterState()
    {
        Debug.Log("Entered Dodge State");

        if (fighter.moveSet == null || fighter.moveSet.dodgeDisplacement == null)
        {
            Debug.LogWarning(
                $"{fighter.name}: No tiene un desplazamiento de Dodge configurado."
            );

            fighter.ChangeState(fighter.IdleState);
            return;
        }

        Vector3 inputDir = fighter.MovementInput;

        if (inputDir == Vector3.zero || inputDir.sqrMagnitude < 0.01f) {
            dodgeDirection = -fighter.transform.forward;
            fighter.animator.CrossFade("Backflip", 0.2f);

        }
        else {
            dodgeDirection = inputDir.normalized;
            fighter.animator?.SetTrigger("Dash");
        }

        bool displacementStarted =
                   fighter.Movement.StartDisplacement(
                       fighter.moveSet.dodgeDisplacement,
                       dodgeDirection
                   );

        if (!displacementStarted)
        {
            fighter.ChangeState(fighter.IdleState);
            return;
        }


    }

    public override void UpdateState()
    {
        if (!fighter.Movement.Displacement.IsActive)
        {
            fighter.ChangeState(fighter.IdleState);
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }
}