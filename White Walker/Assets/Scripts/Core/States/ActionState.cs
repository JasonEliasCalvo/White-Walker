using UnityEngine;

public class ActionState : BaseState
{
    private ActionData currentAction;
    public ActionData CurrentAction => currentAction;

    public ActionState(FighterEntity fighter) : base(fighter) { }

    public void SetAction(ActionData action)
    {
        currentAction = action;
    }

    public override void EnterState()
    {
        if (currentAction == null)
        {
            Debug.LogWarning(
                $"{fighter.name}: ActionState entró sin ActionData."
            );

            fighter.ChangeState(fighter.LocomotionState);
            return;
        }

        Debug.Log(
            $"{fighter.name} → ACTION STATE → {currentAction.actionName}"
        );

        fighter.FighterAnimator.SetActionPlaying(true);
        ExecuteAction();
    }

    private void ExecuteAction()
    {
        if (currentAction == null)
            return;

        if (currentAction.lockHorizontalMovement)
        {
            fighter.Movement.StopHorizontalMovement();
            fighter.Movement.SetHorizontalMovementEnabled(false);
        }

        if (currentAction.displacement != null)
        {
            Vector3 direction = fighter.MovementInput;

            if (direction.sqrMagnitude < 0.01f)
                direction = fighter.transform.forward;

            fighter.Movement.StartDisplacement(
                currentAction.displacement,
                direction.normalized
            );
        }

        if (string.IsNullOrEmpty(currentAction.animationStateName))
        {
            Debug.LogWarning(
                $"{fighter.name}: La acción " +
                $"'{currentAction.actionName}' no tiene animación."
            );

            return;
        }

        int layer =
            fighter.animator.GetLayerIndex("Base Layer");

        if (layer < 0)
        {
            Debug.LogError(
                $"{fighter.name}: No se encontró " +
                $"la capa Base Layer."
            );

            return;
        }

        string statePath = "Base Layer." + currentAction.animationStateName;
        int stateHash = Animator.StringToHash(statePath);

        if (!fighter.animator.HasState(layer, stateHash))
        {
            Debug.LogError(
                            $"{fighter.name}: Animator no tiene " +
                            $"el estado '{statePath}'."
                        );

            return;
        }

        Debug.Log(
            $"{fighter.name} → ACTION STATE → " +
            $"Playing animation: " +
            $"{currentAction.animationStateName}"
        );

        fighter.animator.CrossFade(stateHash, 0.05f, layer);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo info = fighter.animator.GetCurrentAnimatorStateInfo(0);

        // --- VENTANA DE CANCELACIÓN ---
        if (info.normalizedTime >= 0.90f)
        {
           OnActionEnd();
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        fighter.AnimEvent_CloseHitbox(0);
        fighter.AnimEvent_CloseHitbox(1);
        fighter.AnimEvent_CloseHitbox(2);
        fighter.AnimEvent_CloseHitbox(3);
        fighter.AnimEvent_CloseHitbox(4);

        currentAction = null;
        fighter.FighterAnimator.SetActionPlaying(false);
    }

    public void OnActionEnd()
    {
        if (fighter.CurrentState != this)
            return;

        Debug.Log(
            $"{fighter.name} → ACTION END → " +
            $"Returning to LocomotionState"
        );

        fighter.ChangeState(
            fighter.LocomotionState
        );
    }
}
