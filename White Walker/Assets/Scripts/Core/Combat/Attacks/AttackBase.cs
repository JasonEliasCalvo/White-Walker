using System;
using UnityEngine;
using System.Collections.Generic;

public enum ExecutionType { Hit, Grab, Finisher }

public enum CancelType{ None, attack, Dodge, Any }

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class AttackBase : ScriptableObject
{
    [Serializable]
    public class AttackEvent
    {
        public float time; // Tiempo desde inicio del ataque
        public AttackEventType type;
        public int hitboxIndex;
        public int GetTick() => Mathf.RoundToInt(time * 60f);
    }

    public enum AttackEventType
    {
        OpenHitbox,
        CloseHitbox
    }

    [Header("Identidad")]
    public int attackID;
    public string attackName;
    public ExecutionType executionType;
    public AnimationClip animation;
    public float blendTime = 0.08f;

    [Header("Stats Básicos")]
    public int cost;
    public float damage = 10f;
    public float hitStun = 0.5f;
    public float knockbackForce = 5f;
    public bool superArmor;

    [Header("Attack Timeline")]
    public List<AttackEvent> events = new List<AttackEvent>();
    public float comboWindowStart = 0.5f;
    public float TotalDuration
    {
        get
        {
            if (events.Count == 0) return animation.length;
            float lastEventTime = events[events.Count - 1].time;
            return Mathf.Max(lastEventTime, animation.length);
        }
    }

    [Header("Efectos Avanzados")]
    public List<EffectData> effects = new List<EffectData>();

   // Header("Cancel Rules")]
   // public CancelType cancelType;

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;

    public virtual bool CanExecute(FighterEntity user, FighterEntity target)
    {
        if (target == null) return false;

        switch (executionType)
        {
            case ExecutionType.Hit:
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