using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player; 
    public CinemachineFreeLook cinemachineCamera;

    [Header("Sensibilidad de la cámara")]
    public float horizontalSpeed = 300f;
    public float verticalSpeed = 1.5f;

    [Header("Configuración de Rotación")]
    public float rotationSpeed = 2f;      // Velocidad de rotación de la cámara
    public float minVerticalAngle = -40f; // Ángulo mínimo al mirar abajo
    public float maxVerticalAngle = 70f;

    private Vector2 lookInput;

    private void Start()
    {
        // Oculta y bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ajusta la velocidad de rotación de la Cinemachine
        cinemachineCamera.m_XAxis.m_MaxSpeed = horizontalSpeed;
        cinemachineCamera.m_YAxis.m_MaxSpeed = verticalSpeed;
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Leer la entrada del ratón
        lookInput = context.ReadValue<Vector2>();
    }

    private void HandleCameraRotation()
    {
        cinemachineCamera.m_XAxis.m_InputAxisValue = lookInput.x;
        cinemachineCamera.m_YAxis.m_InputAxisValue = lookInput.y;

        // Opcional: Rotar el personaje hacia la dirección de la cámara
        if (player != null)
        {
            Vector3 lookDirection = new Vector3(cinemachineCamera.transform.forward.x, 0, cinemachineCamera.transform.forward.z);
            player.rotation = Quaternion.Slerp(player.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * rotationSpeed);
        }
    }
}

