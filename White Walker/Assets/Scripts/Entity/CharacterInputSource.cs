using UnityEngine;

public abstract class CharacterInputSource : MonoBehaviour
{
    public abstract Vector3 GetMovementDirection();

    public virtual bool ConsumeAttackPressed()
    {
        return false;
    }

    public virtual bool ConsumeDashPressed()
    {
        return false;
    }

    public virtual bool ConsumeJumpPressed()
    {
        return false;
    }
}
