using System;
using UnityEngine;

[Serializable]
public abstract class ActionEffect
{
    public abstract void Execute(GameObject user, ActionContext context);
}

[Serializable]
public class DisplacementEffct : ActionEffect
{
    public DisplacementData displacement;

    public override void Execute(GameObject user, ActionContext context)
    {
        if (displacement == null) return;

        if (user.TryGetComponent<CharacterMovement>(out var movement))
        {
            Vector3 direction = movement.RuntimeData.desiredVelocity;

            if (direction.sqrMagnitude < 0.01f)
                direction = user.transform.forward;

            movement.StartDisplacement(displacement, direction.normalized);
        }
    }
}


[Serializable]
public class DamageEffect : ActionEffect
{
    public AttackData attackData;
    public override void Execute(GameObject user, ActionContext context)
    {

    }
}