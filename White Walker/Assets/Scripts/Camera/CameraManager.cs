using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera manualCamera;
    public CinemachineCamera cinematicCam;

    void Start()
    {
        manualCamera.enabled = true;
        cinematicCam.gameObject.SetActive(false);
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
