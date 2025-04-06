using System;
using UnityEngine; 


public enum EffectTag
{
    None,
    LowAttack,
    BreakGuard,
    AirLauncher,
    Knockdown,
    HighDodge,
    Cancelable,
    HeavyHit,
    Stun,
}

[Serializable]
public class EffectData
{
    public EffectTag tag;
    public float power;
    public float duration;
}