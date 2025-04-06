using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float acceleration = 15f;
    public float deceleration = 20f;
    public float gravity = 8;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.5f;

    [Header("Rotación")]
    public float rotationSpeed = 10f;

    private CharacterController chController;
    private Rigidbody rb;

    private Vector3 velocity;
    private Vector3 inputDir;
    private Vector3 forcedDirection = Vector3.zero;

    private bool usePhysics;
    private bool isDashing;
    private bool isForcedRotation = false;

    private void Start()
    {
        chController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (!usePhysics)
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                StartCoroutine(PerformDash());
            }
        }
    }

    private void HandleMovement()
    {
        if (isDashing) return; 

        inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputDir.magnitude > 0.1f)
        {
            Vector3 moveDir = GetWorldMovementDirection(inputDir);
            velocity = Vector3.MoveTowards(velocity, moveDir * moveSpeed, acceleration * Time.deltaTime);
            RotateCharacter(moveDir);
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (!chController.isGrounded)
        {
            velocity.y += Physics.gravity.y * gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -1f;
        }

        chController.Move(velocity * Time.deltaTime);
    }

    private Vector3 GetWorldMovementDirection(Vector3 inputDirection)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        return (forward * inputDirection.z + right * inputDirection.x).normalized;
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        Vector3 dashDirection = inputDir.magnitude > 0.1f ? GetWorldMovementDirection(inputDir) : -transform.forward;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            chController.Move(dashDirection * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }

    private void RotateCharacter(Vector3 direction)
    {
        if (isForcedRotation && forcedDirection != Vector3.zero)
        {
            direction = forcedDirection;
        }

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void ApplyForce(Vector3 direction, float force)
    {
        if (usePhysics) return;

        usePhysics = true;
        chController.enabled = false;

        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);

        StartCoroutine(RecoverControl());
    }

    private IEnumerator RecoverControl()
    {
        yield return new WaitForSeconds(0.5f);

        rb.isKinematic = true;
        chController.enabled = true;
        usePhysics = false;
    }

    public void ForceDirectionTo(Vector3 direction)
    {
        forcedDirection = direction.normalized;
        isForcedRotation = true;
    }

    public void ResetForcedDirection()
    {
        isForcedRotation = false;
        forcedDirection = Vector3.zero;
    }
}
