using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterDisplacement : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private bool isActive;

    private DisplacementData currentData;
    private Vector3 direction;
    private float elapsedTime;
    private Vector3 previousOffset;

    public bool IsActive => isActive;

    public bool BlocksHorizontalMovement =>
        isActive &&
        currentData != null &&
        currentData.blockHorizontalMovement;

    public bool UsesGravity =>
        !isActive ||
        currentData == null ||
        currentData.applyGravityDuringDisplacement;

    public bool StartDisplacement( DisplacementData data,Vector3 displacementDirection)
    {
        if (data == null)
        {
            Debug.LogWarning(
                $"{name}: Se intentó iniciar un desplazamiento sin DisplacementData.",
                this
            );

            return false;
        }

        if (data.displacementType != DisplacementType.Vertical)
        {
            displacementDirection.y = 0f;

            if (displacementDirection.sqrMagnitude < 0.0001f)
            {
                Debug.LogWarning( $"{name}: El desplazamiento necesita una dirección válida.",
                    this
                );

                return false;
            }

            direction = displacementDirection.normalized;
        }
        else
            direction = Vector3.zero;

        currentData = data;
        elapsedTime = 0f;
        previousOffset = Vector3.zero;
        isActive = true;

        return true;
    }

    public void StopDisplacement()
    {
        isActive = false;
        currentData = null;
        elapsedTime = 0f;
        previousOffset = Vector3.zero;
        direction = Vector3.zero;
    }

    public Vector3 SimulateDisplacement(float deltaTime)
    {
        if (!isActive || currentData == null)
            return Vector3.zero;

        elapsedTime += deltaTime;

        float normalizedTime = Mathf.Clamp01(
                elapsedTime / currentData.duration);

        Vector3 currentOffset = CalculateOffset(normalizedTime);
        Vector3 frameDelta = currentOffset - previousOffset;
        previousOffset = currentOffset;

        if (normalizedTime >= 1f)
        {
            isActive = false;
            currentData = null;
        }

        return frameDelta;
    }

    private Vector3 CalculateOffset( float normalizedTime)
    {
        float progress = currentData.progressCurve.Evaluate(normalizedTime);

        switch (currentData.displacementType)
        {
            case DisplacementType.Linear:
                return direction * currentData.distance * progress;

            case DisplacementType.Vertical:
                return Vector3.up * currentData.verticalDistance * progress;

            case DisplacementType.Parabolic:
                Vector3 horizontalOffset = direction * currentData.distanceForParabola * progress;

                float verticalProgress = 
                    4f * normalizedTime * (1f - normalizedTime);

                Vector3 verticalOffset =
                    Vector3.up * currentData.height * verticalProgress;

                return horizontalOffset + verticalOffset;

            default:
                return Vector3.zero;
        }
    }
}