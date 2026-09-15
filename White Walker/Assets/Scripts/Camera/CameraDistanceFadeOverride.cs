using UnityEngine;

public class CameraDistanceFadeOverride : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinnedMeshRenderer playerRenderer;

    [SerializeField, Tooltip("Asigna aquí la Main Camera")]
    private Transform mainCameraTransform;

    [Header("Settings")]
    [SerializeField] private float startFadeDistance = 1.05f;
    [SerializeField] private float fullTransparentDistance = 0.3f;

    private Material playerMaterialInstance;

    private void Start()
    {
        if (playerRenderer != null)
        {
            // Clona el material para no afectar el asset original
            playerMaterialInstance = playerRenderer.material;
        }
    }

    private void LateUpdate()
    {
        if (playerMaterialInstance != null && mainCameraTransform != null)
        {
            playerMaterialInstance.SetVector("_TargetPosition", mainCameraTransform.position);
            playerMaterialInstance.SetFloat("_MaxDistance", startFadeDistance);
            playerMaterialInstance.SetFloat("_MinDistance", fullTransparentDistance);
        }
    }
}