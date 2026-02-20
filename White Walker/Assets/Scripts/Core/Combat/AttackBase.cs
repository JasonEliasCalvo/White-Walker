using System;
using UnityEngine;
using System.Collections.Generic;

public enum StyleType { Unarmed, Armed }

public enum ExecutionType { Hitbox, Grab, Finisher }

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class AttackBase : ScriptableObject
{
    [Header("Identidad")]
    public int cost;
    public int attackID;
    public string attackName;
    public StyleType category;
    public ExecutionType executionType;
    public AnimationClip animation;

    [Header("Stats Básicos")]
    public float damage = 10f;
    public float hitStun = 0.5f;
    public float knockbackForce = 5f;

    [Header("Frame Data")]
    public float startupTime;
    public float activeTime;
    public float recoveryTime;

    [Header("Efectos Avanzados")]
    public List<EffectData> effects = new List<EffectData>();

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;

    [Header("Swap Behavior")]
    public bool spawnGhostOnSwap = false;

    public float TotalDuration => startupTime + activeTime + recoveryTime;

    public virtual bool CanExecute(FighterEntity user, FighterEntity target)
    {
        if (target == null) return false;

        switch (executionType)
        {
            case ExecutionType.Hitbox:
                return true;

            case ExecutionType.Grab:
                return target != null && target.IsStunned;

            case ExecutionType.Finisher:
                return target != null && target.IsVulnerable;

            default:
                return false;
        }
    }
}

public enum EffectTag { 
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