using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo de cámara")]
    public Transform tpTarget;
    public Camera cam;

    [Header("Orientation")]
    public Transform orientation;

    [Header("Visibilidad de Jugador")]
    public bool disablePlayerMesh = true;
    public GameObject playerMesh;

    [Header("Ajustes de Cámara Sobre el Hombro")]
    public float verticalLimit = 72f;
    public float maxDistance = 7f;
    public float minDistance = 1.5f;

    [Tooltip("Desplazamiento lateral y vertical de la cámara relativo al jugador")]
    public Vector3 shoulderOffset = new Vector3(0.8f, 0.5f, 0f);

    public int zoomVelocity = 300;
    public float zoomSmooth = 0.1f;
    public Vector2 sensitivity = new Vector2(1, 1);

    [Header("Layer de colisión")]
    public LayerMask collisionMask;
    public float collisionCushion = 0.2f;

    private Vector2 angle = new Vector2(0, 0);
    private new Camera camera;
    private float defaultDistance;
    private float newDistance;

    void Start()
    {
        if (disablePlayerMesh && playerMesh != null)
            playerMesh.SetActive(true);

        defaultDistance = (maxDistance + minDistance) / 2;
        newDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camera = cam.GetComponent<Camera>();

        Vector3 angles = transform.eulerAngles;
        angle.x = angles.y;
        angle.y = angles.x;
    }

    void Update()
    {
        float hor = Input.GetAxis("Mouse X");
        float ver = Input.GetAxis("Mouse Y");

        angle.x += hor * sensitivity.x;
        angle.y -= ver * sensitivity.y;

        // Zoom
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            newDistance -= scrollDelta * zoomVelocity * Time.deltaTime;
            newDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        }
        defaultDistance = Mathf.Lerp(defaultDistance, newDistance, zoomSmooth);
    }

    void LateUpdate()
    {
        if (tpTarget == null) return;

        Quaternion camRotation = Quaternion.Euler(angle.y, angle.x, 0);

        Vector3 targetPivot = tpTarget.position + (camRotation * shoulderOffset);
        Vector3 desiredCameraPos = targetPivot - (camRotation * Vector3.forward * defaultDistance);

        // Colisiones
        float currentDistance = defaultDistance;
        Vector3 directionToCamera = (desiredCameraPos - targetPivot).normalized;

        if (Physics.SphereCast(targetPivot, collisionCushion, directionToCamera, out RaycastHit hit, defaultDistance, collisionMask))
        {
            currentDistance = hit.distance;
        }

        transform.position = targetPivot + (directionToCamera * currentDistance);
        transform.rotation = camRotation;

        // ORIENTATION (horizontal)
        if (orientation != null)
        {
            Vector3 forwardFlat = transform.forward;
            forwardFlat.y = 0;
            forwardFlat.Normalize();
            orientation.forward = forwardFlat;

            orientation.localRotation = Quaternion.Euler(orientation.localEulerAngles.x, orientation.localEulerAngles.y, 0);
        }
    }
}