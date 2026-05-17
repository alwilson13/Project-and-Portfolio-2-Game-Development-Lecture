using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDamage
{
    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject damageScreen;

    [Header("Health")]
    [SerializeField] private int maxHP = 5;
    [SerializeField] private int currentHP;

    [Header("Boost")]
    [SerializeField] private float maxBoost = 100f;
    [SerializeField] private float currentBoost = 100f;
    [SerializeField] private float boostRegenRate = 20f;
    [SerializeField] private float boostRegenDelay = 0.5f;
    [SerializeField] private Image boostBarFill;

    [Header("KnockBack")]
    [SerializeField] private float knockbackForce = 3f;

    [Header("Future Stats")]
    public float BoostAmount = 100f;
    public float Armor = 0f;
    public float Acceleration = 8f;
    public float Speed = 10f;
    public float Handling = 4f;
    public float PhysicalAttackPower = 1f;
    public float RangedPower = 1f;
    public float ElementalPower = 1f;
    public float JumpPower = 10f;
    public float Weight = 1f;

    private CharacterController controller;
    private float lastBoostUseTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentHP = maxHP;
        UpdateHealthUI();

        currentBoost = maxBoost;
        UpdateBoostUI();
    }

    private void Update()
    {
        RegenerateBoost();
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.Max(1, amount - Mathf.RoundToInt(Armor));

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (controller != null)
        {
            Vector3 knockDirection = -transform.forward;

            controller.Move(knockDirection * knockbackForce);
        }

        UpdateHealthUI();

        if (damageScreen != null)
        {
            StartCoroutine(FlashDamageScreen());
        }

        if (currentHP <= 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.YouLose();
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHP / maxHP;
        }
    }

    private IEnumerator FlashDamageScreen()
    {
        damageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageScreen.SetActive(false);
    }

    public bool HasBoost(float amount)
    {
        return currentBoost >= amount;
    }

    public bool UseBoost(float amount)
    {
        if (currentBoost < amount)
            return false;

        currentBoost -= amount;
        lastBoostUseTime = Time.time;
        UpdateBoostUI();
        return true;
    }

    private void RegenerateBoost()
    {
        if (Time.time < lastBoostUseTime + boostRegenDelay)
            return;

        currentBoost += boostRegenRate * Time.deltaTime;
        currentBoost = Mathf.Clamp(currentBoost, 0f, maxBoost);
        UpdateBoostUI();
    }

    private void UpdateBoostUI()
    {
        if (boostBarFill != null)
            boostBarFill.fillAmount = currentBoost / maxBoost;
    }
}