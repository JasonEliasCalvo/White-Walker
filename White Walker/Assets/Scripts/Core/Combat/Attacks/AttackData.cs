using Combat;
using System.Collections.Generic;
using UnityEngine;

public enum ExecutionType { Hit, Grab, Finisher }

[CreateAssetMenu(fileName = "New Frame Attack", menuName = "Combat/Frame Based Attack")]
public class AttackData : ScriptableObject
{
    [Header("Identidad")]
    public int attackID;
    public string attackName;
    public ExecutionType executionType;
    public AnimationClip animation;
    public float blendTime = 0.08f;

    [Header("Atributos Globales")]
    public int cost;
    public bool superArmor;

    [Tooltip("Duración total del estado de ataque en Frames (Ej: 60 = 1 segundo)")]
    public int totalFrames = 60;

    [Header("Timeline de Acciones")]
    public List<AttackAction> actions = new();

    [Header("Ventanas de Cancelación")]
    public List<CancelWindow> cancelWindows = new List<CancelWindow>();

    [Header("VFX & SFX")]
    public GameObject hitParticle;
    public AudioClip hitSound;

    public virtual bool CanExecute(FighterEntity user, FighterEntity target)
    {
        if (target == null) return false;

        switch (executionType)
        {
            case ExecutionType.Hit: return true;
            case ExecutionType.Grab: return target.IsStunned;
            case ExecutionType.Finisher: return target.IsVulnerable;
            default: return false;
        }
    }
    public void ResetActions()
    {
        foreach (var action in actions)
        {
            // resetea estado interno
            if (action is AttackAction a)
            {
                // hack simple
                typeof(AttackAction)
                    .GetField("hasStarted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(a, false);
            }
        }
    }
}