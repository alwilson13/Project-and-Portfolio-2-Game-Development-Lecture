using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        MeleeChaser,
        RangedShooter,
        Exploder,
        FlyerMelee,
        FlyerShooter
    }

    [Header("Enemy Type")]
    [SerializeField] private EnemyType enemyType = EnemyType.MeleeChaser;

    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform playerTarget;

    [Header("Fallback Values")]
    [SerializeField] private float fallbackMoveSpeed = 3.5f;
    [SerializeField] private float fallbackDetectionRange = 15f;
    [SerializeField] private float fallbackAttackRange = 2f;
    [SerializeField] private float fallbackFaceTargetSpeed = 8f;
    [SerializeField] private float fallbackAttackRate = 1.5f;
    [SerializeField] private int fallbackDamage = 1;

    [Header("Ranged Weapon")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private bool fireAllPoints = false;

    [Header("Exploder Settings")]
    [SerializeField] private float explodeDelay = 1.25f;
    [SerializeField] private Renderer exploderRenderer;
    [SerializeField] private Color warningColor = Color.white;

    private bool playerInExplosionRadius;
    private bool isExploding;
    private float explodeTimer;
    private Color originalColor;

    private EnemyStats enemyStats;

    private float moveSpeed;
    private float detectionRange;
    private float attackRange;
    private float faceTargetSpeed;
    private float attackRate;
    private int damage;

    private float attackTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyStats = GetComponent<EnemyStats>();

        if (enemyStats != null)
        {
            moveSpeed = enemyStats.MoveSpeed;
            detectionRange = enemyStats.DetectionRange;
            attackRange = enemyStats.AttackRange;
            faceTargetSpeed = enemyStats.FaceTargetSpeed;
            attackRate = enemyStats.AttackRate;
            damage = enemyStats.Damage;
        }
        else
        {
            moveSpeed = fallbackMoveSpeed;
            detectionRange = fallbackDetectionRange;
            attackRange = fallbackAttackRange;
            faceTargetSpeed = fallbackFaceTargetSpeed;
            attackRate = fallbackAttackRate;
            damage = fallbackDamage;
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.9f;
    }

    private void Start()
    {
        if (playerTarget == null && GameManager.instance != null && GameManager.instance.player != null)
        {
            playerTarget = GameManager.instance.player.transform;
        }

        if (exploderRenderer != null)
        {
            originalColor = exploderRenderer.material.color;
        }

        attackTimer = attackRate;
    }

    private void Update()
    {
        if (playerTarget == null)
            return;

        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > detectionRange)
        {
            agent.ResetPath();
            return;
        }

        FaceTarget();

        switch (enemyType)
        {
            case EnemyType.MeleeChaser:
                RunMeleeChaser(distanceToPlayer);
                break;

            case EnemyType.RangedShooter:
                RunRangedShooter(distanceToPlayer);
                break;

            case EnemyType.Exploder:
                RunExploder(distanceToPlayer);
                break;

            case EnemyType.FlyerMelee:
                RunFlyerMelee(distanceToPlayer);
                break;

            case EnemyType.FlyerShooter:
                RunFlyerShooter(distanceToPlayer);
                break;
        }
    }

    private void RunMeleeChaser(float distanceToPlayer)
    {
        agent.SetDestination(playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            TryMeleeAttack();
        }
    }

    private void RunRangedShooter(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(playerTarget.position);
        }
        else
        {
            agent.ResetPath();
            TryShootAttack();
        }
    }

    private void RunExploder(float distanceToPlayer)
    {
        if (playerInExplosionRadius)
        {
            StartExplodeCountdown();
            return;
        }

        if (isExploding || explodeTimer > 0f)
        {
            ResetExplodeCountdown();
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }
    }

    public void SetPlayerInExplosionRadius(bool value)
    {
        playerInExplosionRadius = value;

        if (!value)
        {
            ResetExplodeCountdown();
        }
    }

    private void StartExplodeCountdown()
    {
        isExploding = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        explodeTimer += Time.deltaTime;

        if (exploderRenderer != null)
        {
            float flash = Mathf.PingPong(Time.time * 8f, 1f);
            exploderRenderer.material.color = Color.Lerp(originalColor, warningColor, flash);
        }

        if (explodeTimer >= explodeDelay)
        {
            Explode();
        }
    }

    private void ResetExplodeCountdown()
    {
        isExploding = false;
        explodeTimer = 0f;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        if (exploderRenderer != null)
        {
            exploderRenderer.material.color = originalColor;
        }
    }

    private void Explode()
    {
        IDamage damageTarget = playerTarget.GetComponent<IDamage>();

        if (damageTarget != null)
        {
            damageTarget.TakeDamage(damage);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateGameGoal(-1);
        }

        Destroy(gameObject);
    }

    private void RunFlyerMelee(float distanceToPlayer)
    {
        // Placeholder for now. Uses normal NavMesh behavior until flyer movement is added.
        RunMeleeChaser(distanceToPlayer);
    }

    private void RunFlyerShooter(float distanceToPlayer)
    {
        // Placeholder for now. Uses normal ranged behavior until flyer movement is added.
        RunRangedShooter(distanceToPlayer);
    }

    private void TryMeleeAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            attackTimer = 0f;

            IDamage damageTarget = playerTarget.GetComponent<IDamage>();

            if (damageTarget != null)
            {
                damageTarget.TakeDamage(damage);
            }

            Debug.Log(gameObject.name + " melee attacked player.");
        }
    }

    private void TryShootAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            attackTimer = 0f;

            if (projectilePrefab != null && firePoints != null && firePoints.Length > 0)
            {
                if (fireAllPoints)
                {
                    foreach (Transform point in firePoints)
                    {
                        FireProjectile(point);
                    }
                }
                else
                {
                    FireProjectile(firePoints[Random.Range(0, firePoints.Length)]);
                }
            }

            Debug.Log(gameObject.name + " fired projectile.");
        }
    }

    private void FireProjectile(Transform point)
    {
        if (point == null || playerTarget == null)
            return;

        Vector3 shootDirection = playerTarget.position - point.position;
        shootDirection.y += 0.5f;
        shootDirection.Normalize();

        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        Instantiate(projectilePrefab, point.position, shootRotation);
    }

    private void FaceTarget()
    {
        Vector3 direction = playerTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            faceTargetSpeed * Time.deltaTime
        );
    }
}