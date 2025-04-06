using System.Collections.Generic;
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

public abstract class AttackBase : ScriptableObject
{
    [Header("Attack Data")]
    public int id;
    public string attackName;
    public string attackerAnimation;
    public string defenderAnimation;
    public float damage;
    public float speed;
    public float angle;

    [Header("Effects")]
    public List<EffectTag> effects;
}
