using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo de cámara")]
    public Transform tpTarget;
    public Camera cam;

    [Header("Orientation (para movimiento del jugador)")]
    public Transform orientation;

    [Header("Visibilidad de Jugador")]
    public bool disablePlayerMesh = true;
    public GameObject playerMesh;

    [Header("Ajustes de Cámara")]
    public float verticalLimit = 72f;
    public float maxDistace = 7f;
    public float minDistance = 2f;
    public int zoomVelocity = 300;
    public float zoomSmoth = 0.1f;
    public float rotationSmooth = 0.1f;
    public Vector2 sensitivity = new Vector2(0.1f, 0.1f);

    [Header("Inclinación")]
    public float tiltAmount = 10f;
    public float tiltSmooth = 10f;
    private float currentTilt = 0f;

    [Header("Variables Internas")]
    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad, 0);
    private new Camera camera;
    private Vector2 nearPlaneSize;
    private Transform follow;
    private float defaultDistance;
    private float newDistance;

    void Start()
    {
        follow = tpTarget;

        if (disablePlayerMesh)
            playerMesh.SetActive(true);

        defaultDistance = (maxDistace + minDistance) / 2;
        newDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        camera = cam.GetComponent<Camera>();

        CalculateNearPlaneSize();
    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.nearClipPlane;
        float width = height * camera.aspect;

        nearPlaneSize = new Vector2(width, height);
    }

    private Vector3[] GetCameraCollisionPoints(Vector3 direction)
    {
        Vector3 position = follow.position;
        Vector3 center = position + direction * (camera.nearClipPlane + 0.4f);

        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        return new Vector3[]
        {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        if (mouseDelta.x != 0)
            angle.x += mouseDelta.x * Mathf.Deg2Rad * sensitivity.x;

        if (mouseDelta.y != 0)
        {
            angle.y += mouseDelta.y * Mathf.Deg2Rad * sensitivity.y;
            angle.y = Mathf.Clamp(angle.y, -verticalLimit * Mathf.Deg2Rad, verticalLimit * Mathf.Deg2Rad);
        }

        // Zoom
        float scrollDelta = Mouse.current.scroll.y.ReadValue();

        if (scrollDelta > 0)
            newDistance -= 0.1f * (Time.deltaTime * zoomVelocity);
        else if (scrollDelta < 0)
            newDistance += 0.1f * (Time.deltaTime * zoomVelocity);

        newDistance = Mathf.Clamp(newDistance, minDistance, maxDistace);
        defaultDistance = Mathf.Lerp(defaultDistance, newDistance, zoomSmoth);
    }

    void LateUpdate()
    {
        // Dirección desde la cámara hacia el jugador
        Vector3 direction = new Vector3(
            Mathf.Cos(angle.x) * Mathf.Cos(angle.y),
            -Mathf.Sin(angle.y),
            -Mathf.Sin(angle.x) * Mathf.Cos(angle.y)
        );

        // Colisiones
        RaycastHit hit;
        float distance = defaultDistance;
        Vector3[] points = GetCameraCollisionPoints(direction);

        foreach (Vector3 point in points)
        {
            if (Physics.Raycast(point, direction, out hit, defaultDistance))
                distance = Mathf.Min((hit.point - follow.position).magnitude, distance);
        }

        // Posición de la cámara
        transform.position = follow.position + direction * distance;

        // Rotación hacia el jugador
        transform.rotation = Quaternion.LookRotation(follow.position - transform.position);

        // ORIENTATION (horizontal)
        if (orientation != null)
        {
            Vector3 forwardFlat = transform.forward;
            forwardFlat.y = 0;
            forwardFlat.Normalize();

            orientation.forward = forwardFlat;
        }

        // --------------------------------------------------------
        // INCLINACIÓN SUAVE TIPO GOD OF WAR (vertical del cuerpo)
        // --------------------------------------------------------

        float normalizedY = angle.y / (verticalLimit * Mathf.Deg2Rad);  // de -1 a 1
        float targetTilt = normalizedY * tiltAmount;                    // grados

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmooth);

        orientation.localRotation = Quaternion.Euler(currentTilt, orientation.localEulerAngles.y, 0);
    }
}