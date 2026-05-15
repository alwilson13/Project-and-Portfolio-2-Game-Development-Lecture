using UnityEngine;

public class PlayerSprayCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Spray Settings")]
    [SerializeField] private float sprayRange = 25f;
    [SerializeField] private float sprayRate = 0.2f;
    [SerializeField] private LayerMask sprayLayer; 

    private float sprayTimer;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        sprayTimer += Time.deltaTime;

        Debug.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * sprayRange,
            Color.magenta
        );

        if (Input.GetButton("Fire1") && sprayTimer >= sprayRate)
        {
            ShootSpray();
        }

        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;
    }

    private void ShootSpray()
    {
        Vector3 sprayDirection =
        playerCamera.transform.forward +
        Random.insideUnitSphere * 0.03f;

        sprayDirection.Normalize();
        sprayTimer = 0f;

        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, sprayDirection, out hit, sprayRange, sprayLayer))
        {
            Debug.Log("Spray hit: " + hit.collider.name);

            IDamage damageTarget = hit.collider.GetComponentInParent<IDamage>();

            if (damageTarget != null)
            {
                damageTarget.TakeDamage(1);
            }
        }
    }
}