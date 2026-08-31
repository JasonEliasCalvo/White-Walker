using UnityEngine;

public enum ActionType
{
    Attack,
    Jump,
    Dash,
    Special,
    Other
}

public enum ActionAnimationMode
{
    Single,
    LocomotionDriven,
    Sequence
}

[CreateAssetMenu(fileName = "New Action", menuName = "Combat/Action")]
public class ActionData : ScriptableObject
{
    [Header("Identidad")]
    public string actionName;
    public ActionType actionType;

    [Header("Input")]
    public InputCommandType inputType;
    public InputDirection inputDirection;

    [Header("Animation")]
    public string animationStateName;

    [Header("Movimiento")]
    public bool useMovement = true;
    public bool lockHorizontalMovement = false;
    public DisplacementData displacement;

    [Header("Combate")]
    public float damage = 0f;
    public float hitStun = 0f;
    public float knockbackForce = 0f;

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;
    public AudioClip swingSound;
    public GameObject swingParticlePrefab;
}
