using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform followTarget;

    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 150f;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float shoulderOffset = 0.75f;
    [SerializeField] private float verticalOffset = 1.5f;
    [SerializeField] private float followSmoothTime = 0.06f;

    [Header("Pitch Clamp")]
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 75f;

    private float yaw;
    private float pitch = 15f;
    private Vector3 followVelocity;

    public Vector3 AimDirection => transform.forward;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (followTarget != null)
            yaw = followTarget.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (followTarget == null)
            return;

        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition =
            followTarget.position
            - rotation * Vector3.forward * distance
            + rotation * Vector3.right * shoulderOffset
            + Vector3.up * verticalOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            followSmoothTime
        );

        transform.rotation = rotation;
    }
}