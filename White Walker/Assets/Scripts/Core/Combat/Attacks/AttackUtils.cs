using System;
using System.Linq;
using UnityEngine;

public static class AttackUtils
{
    public static bool HasEffect(AttackBase attack, EffectTag tag)
    {
        return attack.effects.Any(e => e.tag == tag);
    }

    public static float GetEffectPower(AttackBase attack, EffectTag tag)
    {
        foreach (var effect in attack.effects)
            if (effect.tag == tag)
                return effect.power;
        return 0f;
    }

    public static float GetEffectDuration(AttackBase attack, EffectTag tag)
    {
        foreach (var effect in attack.effects)
            if (effect.tag == tag)
                return effect.duration;
        return 0f;
    }
}

public enum EffectTag
{
    None,
    BreakGuard,
    AirLauncher,
    Knockdown,
    HeavyHit,
    LowAttack,
}

[Serializable]
public struct EffectData
{
    public EffectTag tag;
    public float power;
    public float duration;
}