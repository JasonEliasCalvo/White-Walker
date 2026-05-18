using System;
using UnityEngine;
using static UnityEngine.Audio.IAudioGenerator;

public enum ExecutionType { Movement, Light, Heavy, Grab, Special, Projectile };

[CreateAssetMenu(fileName = "ActionData", menuName = "Fighter/ActionData", order = 1)]
public class ActionData : ScriptableObject
{
    [Header("Identidad")]
    public int actionID;
    public string actionName;
    public string animationName;
    public AnimationClip animation;
    public float blendTime = 0.08f;
    public ExecutionType executionType;

    [Tooltip("Duración total del estado de ataque en Frames")]
    public int numberOfFrames = 60;

    [Header("Atributos Globales")]
    public int cost;
    public bool superArmor;
    public bool alwaysCancelable;
    public bool airOkay;
    public bool stopOnLanding;
    [Range(1, 15)] public int bufferLimit = 15;

    [Header("Damage & Stun")]
    public int damage = 0;
    public bool knockdown;
    public float pushback = 0f;
    public float hitStun = 0.5f;
    public float knockbackForce = 5f;

    public enum HitAnim { Light, Heavy };
    public HitAnim hitAnim;
    public int hitAdv = 0;
    public int blockAdv = 0;

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;
    public bool playWhiffSound;
    public AudioClip customWhiffSound;
    public int whiffPlayFrame = -1;

    [Header("Throws")]
    public ThrowData throwData;

    [Header("Input")]
    public MotionData[] validMotions;

    [Header("Data")]
    public HurtboxData[] hurtboxes;
    public HitboxData[] hitboxes;
    public MovementData[] movements;
    public CancelsData[] cancels;
    public ProjectileData[] projectiles;
}

[Serializable]
public class MotionData
{
    public string[] inputs;
}

// X = Frame Inicio, Y = Frame Fin

public class HurtboxData
{
    public Vector2Int startEndFrames;
    public bool useBaseCollider;
    public Vector3 offset = Vector3.zero;
    public Vector3 size = Vector3.one;
}

[Serializable]
public class HitboxData
{
    public int hitboxIndex;
    public Vector2Int startEndFrames;
    public Vector3 offset = Vector3.zero;
    public Vector3 size = Vector3.one;
}

[Serializable]
public class MovementData
{
    public enum MovementType { Velocity, Acceleration };
    public Vector2Int startEndFrames;
    public MovementType movementType;
    public Vector3 movementDirection = Vector3.forward;
    public float velocity = 0f;
    public bool setZeroes;
}

[Serializable]
public class CancelsData
{
    public Vector2Int startEndFrames;
    public string inputRequired; // Ej: "5a", "6a", "4a"
    public bool requireHit;    // (¿El ataque debe golpear para cancelar?)
    public ActionData nextAction;
}

[Serializable]
public class ProjectileData
{
    public int frame;
    public Vector2 offset;
    public GameObject projectilePrefab;
}

[Serializable]
public class DamageData
{
    public int frame;
    public int damage;
}