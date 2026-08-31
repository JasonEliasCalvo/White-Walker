public class ActionResolver
{
    public ActionData Resolve(InputCommand command, ActionContext context, MoveSet moveSet)
    {
        if (moveSet == null || moveSet.actions == null)
            return null;

        for (int i = 0; i < moveSet.actions.Count; i++)
        {
            ActionData action = moveSet.actions[i];

            if (action == null)
                continue;

            if (action.inputType != command.type)
                continue;

            if (action.inputDirection != command.direction)
                continue;

            if (!IsContextValid(action, context))
                continue;

            return action;
        }

        return null;
    }
    private bool IsContextValid( ActionData action, ActionContext context)
    {
        return true;
    }
}

public struct ActionContext
{
    public bool isGrounded;
    public bool isAirborne;
    public bool isRunning;

    public ActionData currentAction;

    public ActionContext(
        bool isGrounded,
        bool isRunning,
        ActionData currentAction)
    {
        this.isGrounded = isGrounded;
        this.isAirborne = !isGrounded;
        this.isRunning = isRunning;
        this.currentAction = currentAction;
    }
}