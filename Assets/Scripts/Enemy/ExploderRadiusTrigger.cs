using UnityEngine;

public class ExploderRadiusTrigger : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enemyAI != null)
        {
            enemyAI.SetPlayerInExplosionRadius(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && enemyAI != null)
        {
            enemyAI.SetPlayerInExplosionRadius(false);
        }
    }
}