using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 150f;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2.5f;
    [SerializeField] private float followSmoothTime = 0.08f;
    [SerializeField] private float lookHeight = 1.4f;

    [Header("Vertical Look Clamp")]
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 45f;

    private float yaw;
    private float pitch;
    private Vector3 followVelocity;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        yaw = player.eulerAngles.y;
        pitch = 15f;
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition =
            player.position
            + Vector3.up * height
            - cameraRotation * Vector3.forward * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            followSmoothTime
        );

        transform.LookAt(player.position + Vector3.up * lookHeight);

        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;
    }
}