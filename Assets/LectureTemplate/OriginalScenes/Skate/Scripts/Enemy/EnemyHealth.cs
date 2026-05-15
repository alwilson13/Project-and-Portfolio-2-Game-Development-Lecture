using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private Renderer rend;

    [Header("Enemy Health")]
    [SerializeField] private int hp = 3;

    private Color colorOrig;

    private void Start()
    {
        if (rend == null)
        {
            rend = GetComponentInChildren<Renderer>();
        }

        colorOrig = rend.material.color;

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdateGameGoal(1);
        }
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateGameGoal(-1);
            }

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = colorOrig;
    }
}