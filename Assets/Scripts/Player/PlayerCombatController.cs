using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform projectileFirePoint;

    [Header("Primary / Pistol")]
    [SerializeField] private GameObject playerProjectilePrefab;
    [SerializeField] private float pistolFireRate = 0.35f;

    [Header("Spray Gadget")]
    [SerializeField] private float sprayRange = 25f;
    [SerializeField] private float sprayRate = 0.2f;
    [SerializeField] private float spraySpread = 0.03f;
    [SerializeField] private int sprayDamage = 1;
    [SerializeField] private LayerMask sprayLayer;

    [Header("Skill / Shield")]
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private float shieldCooldown = 10f;

    private float pistolTimer;
    private float sprayTimer;
    private float shieldTimer;
    private float shieldCooldownTimer;
    private bool shieldActive;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        pistolTimer += Time.deltaTime;
        sprayTimer += Time.deltaTime;

        HandlePrimaryFire();
        HandleSprayFire();
        HandleShieldSkill();
    }

    private void HandlePrimaryFire()
    {
        if (Input.GetButton("Fire1") && pistolTimer >= pistolFireRate)
        {
            pistolTimer = 0f;
            FirePlayerProjectile();
        }
    }

    private void HandleSprayFire()
    {
        if (Input.GetButton("Fire2") && sprayTimer >= sprayRate)
        {
            sprayTimer = 0f;
            UseSpray();
        }
    }

    private void HandleShieldSkill()
    {
        if (shieldCooldownTimer > 0f)
            shieldCooldownTimer -= Time.deltaTime;

        if (shieldActive)
        {
            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0f)
            {
                shieldActive = false;

                if (shieldObject != null)
                    shieldObject.SetActive(false);
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && shieldCooldownTimer <= 0f)
        {
            shieldActive = true;
            shieldTimer = shieldDuration;
            shieldCooldownTimer = shieldCooldown;

            if (shieldObject != null)
                shieldObject.SetActive(true);
        }
    }

    private void FirePlayerProjectile()
    {
        if (playerProjectilePrefab == null || projectileFirePoint == null || playerCamera == null)
            return;

        Vector3 aimPoint;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, sprayLayer))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * 100f;
        }

        Vector3 shootDirection = aimPoint - projectileFirePoint.position;
        shootDirection.Normalize();

        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        Instantiate(playerProjectilePrefab, projectileFirePoint.position, shootRotation);
    }

    private void UseSpray()
    {
        if (playerCamera == null)
            return;

        Vector3 sprayDirection =
            playerCamera.transform.forward +
            Random.insideUnitSphere * spraySpread;

        sprayDirection.Normalize();

        Debug.DrawRay(playerCamera.transform.position, sprayDirection * sprayRange, Color.magenta, 0.2f);

        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, sprayDirection, out hit, sprayRange, sprayLayer))
        {
            Debug.Log("Spray hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Shield"))
            {
                Debug.Log("Shield blocked the spray.");
                return;
            }

            IDamage damageTarget = hit.collider.GetComponentInParent<IDamage>();

            if (damageTarget != null)
            {
                damageTarget.TakeDamage(sprayDamage);
            }
        }
    }
}