[System.Serializable]
public class StateEvent
{
    public int startFrame;
    public int endFrame;
    public StateFunctionType function;
}

public enum StateFunctionType
{
    None,
    ActivateHitbox,
    MoveForward,
    ApplyForce,
    EndState,
    EnableCancel,
    DisableCancel
}