using System;
using System.Collections.Generic;
using UnityEngine;

public enum StyleType { Unarmed, Armed }

public class AttackBase : ScriptableObject
{
    [Header("Identidad")]
    public int cost;
    public int attackID;
    public string attackName;
    public StyleType category;
    public AnimationClip animation;

    [Header("Stats Básicos")]
    public float damage = 10f;
    public float hitStun = 0.5f;
    public float knockbackForce = 5f;
    public float duration;

    [Header("Efectos Avanzados")]
    public List<EffectData> effects = new List<EffectData>();

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;

    [Header("Swap Behavior")]
    public bool spawnGhostOnSwap = false;
}

public enum EffectTag { 
    None,
    BreakGuard,
    AirLauncher,
    Knockdown,
    Stun,
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