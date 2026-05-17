using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private Renderer rend;

    [Header("Fallback Health")]
    [SerializeField] private int fallbackHP = 3;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 4f;

    private int currentHP;
    private EnemyStats enemyStats;
    private Color colorOrig;
    private Rigidbody rb;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        currentHP = enemyStats != null ? enemyStats.MaxHP : fallbackHP;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            colorOrig = rend.material.color;

        if (GameManager.instance != null)
            GameManager.instance.UpdateGameGoal(1);
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            Vector3 knockDirection = (transform.position - playerObj.transform.position).normalized;
            knockDirection.y = 0f;

            transform.position += knockDirection * knockbackForce;
        }

        if (currentHP <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        if (rend == null)
            yield break;

        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = colorOrig;
    }
}