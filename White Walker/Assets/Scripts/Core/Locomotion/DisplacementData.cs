using UnityEngine;

public enum DisplacementType
{
    Linear,
    Vertical,
    Parabolic
}

[CreateAssetMenu(
    fileName = "New Displacement",
    menuName = "Action/Movement/Displacement Data"
)]
public class DisplacementData : ScriptableObject
{
    [Header("Type")]
    public DisplacementType displacementType =
        DisplacementType.Linear;

    [Header("Timing")]
    [Min(0.01f)]
    public float duration = 0.25f;

    [Header("Horizontal")]
    public float distance = 2f;

    [Header("Vertical")]
    public float verticalDistance = 2f;

    [Header("Parabolic")]
    public float height = 2f;
    public float distanceForParabola = 2f;

    [Header("Progress")]
    public AnimationCurve progressCurve =
        AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Behaviour")]
    public bool blockHorizontalMovement = true;
    public bool applyGravityDuringDisplacement = false;
}