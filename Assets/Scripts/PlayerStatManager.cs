using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    // define different stats as public, so they can be visible in Unity Inspector
    public Stat maxHealth = new Stat { statName = "Health", level = 0, baseValue = 100f, addValue = 25f };
    public Stat armor = new Stat { statName = "Armor", level = 0, baseValue = 0f, addValue = 5f };
    public Stat movementSpeed = new Stat { statName = "Movement Speed", level = 0, baseValue = 2f, addValue = 0.5f };
    public Stat sprintSpeed = new Stat { statName = "Sprint Speed", level = 0, baseValue = 5f, addValue = 0.5f };
    public Stat stamina = new Stat { statName = "Stamina", level = 0, baseValue = 5f, addValue = 0.5f };
    public Stat attackDamage = new Stat { statName = "Attack Damage", level = 0, baseValue = 50f, addValue = 5f };
    public Stat attackRange = new Stat { statName = "Attack Range", level = 0, baseValue = 5f, addValue = 1f };
    public Stat attackSpeed = new Stat { statName = "Attack Speed", level = 0, baseValue = 1f, addValue = 0.2f };
    public float currentHealth = 0;
    public int currentTokens = 0;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private HitEffectUI hitEffectUI;

    // references for values and levels to display in UI
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private TextMeshProUGUI armorLevelText;
    [SerializeField] private TextMeshProUGUI movementSpeedLevelText;
    [SerializeField] private TextMeshProUGUI sprintSpeedLevelText;
    [SerializeField] private TextMeshProUGUI staminaLevelText;
    [SerializeField] private TextMeshProUGUI attackDamageLevelText;
    [SerializeField] private TextMeshProUGUI attackRangeLevelText;
    [SerializeField] private TextMeshProUGUI attackSpeedLevelText;
    [SerializeField] private TextMeshProUGUI currentTokensText;

    // references to upgrade icons
    [SerializeField] private Image armorUpgradeIcon;
    [SerializeField] private Image movementSpeedUpgradeIcon;
    [SerializeField] private Image sprintSpeedUpgradeIcon;
    [SerializeField] private Image staminaUpgradeIcon;
    [SerializeField] private Image attackDamageUpgradeIcon;
    [SerializeField] private Image attackRangeUpgradeIcon;
    [SerializeField] private Image attackSpeedUpgradeIcon;

    // reference to handle player input
    [SerializeField] private PlayerInputHandler playerInput;

    // reference for health slider
    [SerializeField] private Slider healthSlider;

    private Dictionary<string, (Stat stat, TextMeshProUGUI text)> statsDictionary;
    private void Awake()
    {
        // intialize health slider
        healthSlider = transform.Find("PlayerStats/Defense/HealthBar").GetComponent<Slider>();
        healthAmountText = transform.Find("PlayerStats/Defense/HealthBar/HealthAmountText").GetComponent<TextMeshProUGUI>();

        currentHealth = maxHealth.Value;

        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth.Value;
        healthSlider.value = currentHealth;

        RefreshTotalHealthUI();

        // initialize UI labels
        armorLevelText = transform.Find("PlayerStats/Defense/Armor/ArmorIcon/ArmorLevelText").GetComponent<TextMeshProUGUI>();
        armorLevelText.text = armor.level.ToString();

        movementSpeedLevelText = transform.Find("PlayerStats/Movement/MovementSpeed/MovementSpeedIcon/MovementSpeedLevelText").GetComponent<TextMeshProUGUI>();
        movementSpeedLevelText.text = movementSpeed.level.ToString();

        sprintSpeedLevelText = transform.Find("PlayerStats/Movement/SprintSpeed/SprintSpeedIcon/SprintSpeedLevelText").GetComponent<TextMeshProUGUI>();
        sprintSpeedLevelText.text = sprintSpeed.level.ToString();

        staminaLevelText = transform.Find("PlayerStats/Movement/Stamina/StaminaIcon/StaminaLevelText").GetComponent<TextMeshProUGUI>();
        staminaLevelText.text = stamina.level.ToString();

        attackDamageLevelText = transform.Find("PlayerStats/Combat/AttackDamage/AttackDamageIcon/AttackDamageLevelText").GetComponent<TextMeshProUGUI>();
        attackDamageLevelText.text = attackDamage.level.ToString();

        attackRangeLevelText = transform.Find("PlayerStats/Combat/AttackRange/AttackRangeIcon/AttackRangeLevelText").GetComponent<TextMeshProUGUI>();
        attackRangeLevelText.text = attackRange.level.ToString();

        attackSpeedLevelText = transform.Find("PlayerStats/Combat/AttackSpeed/AttackSpeedIcon/AttackSpeedLevelText").GetComponent<TextMeshProUGUI>();
        attackSpeedLevelText.text = attackSpeed.level.ToString();

        currentTokensText = transform.Find("PlayerStats/Tokens").GetComponent<TextMeshProUGUI>();
        currentTokensText.text = "Tokens:\n" + currentTokens.ToString();

        hitEffectUI = GetComponent<HitEffectUI>();

        // stats dictionary for later use

        statsDictionary =
            new Dictionary<string, (Stat, TextMeshProUGUI)>
            {
                { "Armor", (armor, armorLevelText) },
                { "Movement Speed", (movementSpeed, movementSpeedLevelText) },
                { "Sprint Speed", (sprintSpeed, sprintSpeedLevelText) },
                { "Stamina", (stamina, staminaLevelText) },
                { "Attack Damage", (attackDamage, attackDamageLevelText) },
                { "Attack Range", (attackRange, attackRangeLevelText) },
                { "Attack Speed", (attackSpeed, attackSpeedLevelText) }
            };
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor.Value, 0);
        currentHealth -= effectiveDamage;
        hitEffectUI.PlayHitEffect();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameManager.EndGame();
        }
        RefreshTotalHealthUI();
    }

    // call this function when the health level is getting updated
    private void UpdateHealthMaxValue()
    {
        currentHealth += maxHealth.addValue;
        healthSlider.maxValue = maxHealth.Value;
        healthSlider.value = currentHealth;
        RefreshTotalHealthUI();
    }

    public void IncreaseLevel(string statName)
    {
        if (statsDictionary.TryGetValue(statName, out var entry))
        {
            if (currentTokens <= entry.stat.level)
                return;

            entry.stat.level += 1;
            entry.text.text = entry.stat.level.ToString();
            IncreaseToken(-entry.stat.level);
        }

        if (statName == "Health")
        {
            UpdateHealthMaxValue();
        }
    }

    public void IncreaseToken(int amount = 1)
    {
        currentTokens += amount;
        currentTokensText.text = "Tokens:\n" + currentTokens.ToString();
        armorUpgradeIcon.gameObject.SetActive(armor.level < currentTokens);
        movementSpeedUpgradeIcon.gameObject.SetActive(movementSpeed.level < currentTokens);
        sprintSpeedUpgradeIcon.gameObject.SetActive(sprintSpeed.level < currentTokens);
        staminaUpgradeIcon.gameObject.SetActive(stamina.level < currentTokens);
        attackDamageUpgradeIcon.gameObject.SetActive(attackDamage.level < currentTokens);
        attackRangeUpgradeIcon.gameObject.SetActive(attackRange.level < currentTokens);
        attackSpeedUpgradeIcon.gameObject.SetActive(attackSpeed.level < currentTokens);
    }

    private void RefreshTotalHealthUI()
    {
        healthSlider.maxValue = maxHealth.Value;
        healthSlider.value = currentHealth;
        healthAmountText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth.Value)}";
    }
}
