using UnityEngine;

public enum InputCommandType
{
    Attack,
    Dash,
    Jump,
    Interact
}

public enum InputDirection
{
    None,
    Forward,
    Back,
    Left,
    Right
}

public struct InputCommand
{
    public InputCommandType type;
    public InputDirection direction;

    public Vector2 rawDirection;

    public float time;

    public InputCommand( InputCommandType type,InputDirection direction,
        Vector2 rawDirection,float time){
        this.type = type;
        this.direction = direction;
        this.rawDirection = rawDirection;
        this.time = time;
    }
}