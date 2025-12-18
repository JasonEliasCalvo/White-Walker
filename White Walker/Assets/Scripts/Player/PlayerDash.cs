using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    private PlayerMovement movement;

    [Header("Dash Settings")]
    public float dashForce = 25f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.5f;

    [Header("Input")]
    [SerializeField] private InputActionReference dash;

    private float cooldownTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        dash.action.Enable();
        dash.action.performed += OnDash;
    }

    private void OnDisable()
    {
        dash.action.performed -= OnDash;
        dash.action.Disable();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (movement.isDashing) return;
        if (cooldownTimer < dashCooldown) return;

        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        cooldownTimer = 0f;
        movement.isDashing = true;
        movement.movementState = MovementState.Dashing;

        Vector3 dashDir = GetDashDirection();

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            controller.Move(dashDir * dashForce * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        movement.isDashing = false;
        movement.movementState = MovementState.Walking;
        movement.ResetHorizontalVelocity();
    }

    private Vector3 GetDashDirection()
    {
        Transform orientation = movement.orientation;

        float h = movement.inputRaw.x;
        float v = movement.inputRaw.z;

        Vector3 inputDir = (orientation.forward * v + orientation.right * h);
        inputDir.y = 0f;

        if (inputDir.sqrMagnitude > 0.01f)
            return inputDir.normalized;

        // Caso 2: sin input -> dash hacia atrás
        return -movement.transform.forward;
    }

}
