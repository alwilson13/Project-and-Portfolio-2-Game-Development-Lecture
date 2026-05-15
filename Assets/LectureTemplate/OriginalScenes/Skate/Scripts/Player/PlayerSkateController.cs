using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSkateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Skate Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float boostSpeed = 12f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private int maxJumps = 1;

    private CharacterController controller;

    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;
    private int jumpCount;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpAndGravity();

        Vector3 finalVelocity = horizontalVelocity + verticalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);

        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        bool sprintHeld = Input.GetButton("Sprint");

        float targetSpeed = sprintHeld ? boostSpeed : maxSpeed;
        Vector3 targetVelocity = moveDirection * targetSpeed;

        float currentAcceleration =
            moveDirection.sqrMagnitude > 0.01f ? acceleration : deceleration;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            currentAcceleration * Time.deltaTime
        );

        if (horizontalVelocity.sqrMagnitude > 0.1f)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float leanAmount = -horizontalInput * 15f;

            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized) * Quaternion.Euler(0f, 0f, leanAmount);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }

    private void HandleJumpAndGravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
            jumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            verticalVelocity.y = jumpForce;
            jumpCount++;
        }

        verticalVelocity.y -= gravity * Time.deltaTime;
    }
}