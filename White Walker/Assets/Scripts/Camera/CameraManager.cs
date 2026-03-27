using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [Header("Targets")]
    public Transform playerTransform;
    public Transform currentEnemy;

    public CameraController basicCam;
    public CombatCameraController combatCam;

    public Camera manualCamera;
    public CinemachineCamera cinematicCam;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Cinematic
    }

    public CameraStyle currentStyle;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        manualCamera.enabled = true;
        SwitchCameraStyle(CameraStyle.Basic);
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchCameraStyle(CameraStyle.Basic);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchCameraStyle(CameraStyle.Combat);
    }

    public Vector3 GetCombatAnchor()
    {
        if (currentEnemy == null) return playerTransform.position;

        // Calculamos el centro exacto entre los dos
        return (playerTransform.position + currentEnemy.position) / 2f;
    }

    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        basicCam.enabled = false;
        combatCam.enabled = false;

        if (newStyle == CameraStyle.Basic) basicCam.enabled = true;
        if (newStyle == CameraStyle.Combat) combatCam.enabled = true;

        currentStyle = newStyle;
        Debug.Log("Camera style switched to: " + newStyle);
    }

    public void PlayCinematic()
    {
        manualCamera.enabled = false;
        cinematicCam.gameObject.SetActive(true);
    }

    public void EndCinematic()
    {
        cinematicCam.gameObject.SetActive(false);
        manualCamera.enabled = true;
    }
}
