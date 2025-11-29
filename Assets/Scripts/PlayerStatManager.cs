using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    // define different stats as public, so they can be visible in Unity Inspector
    public Stat maxHealth = new Stat { statName = "Health", level = 0, baseValue = 100f, addValue = 25f };
    public Stat armor = new Stat { statName = "Armor", level = 0, baseValue = 0f, addValue = 5f };
    public Stat movementSpeed = new Stat { statName = "Movement Speed", level = 0, baseValue = 2f, addValue = 0.5f };
    public Stat sprintSpeed = new Stat { statName = "Sprint Speed", level = 0, baseValue = 10f, addValue = 0.5f };
    public Stat stamina = new Stat { statName = "Stamina", level = 0, baseValue = 5f, addValue = 0.5f };
    public Stat attackDamage = new Stat { statName = "Attack Damage", level = 0, baseValue = 50f, addValue = 5f };
    public Stat attackRange = new Stat { statName = "Attack Range", level = 0, baseValue = 5f, addValue = 1f };
    public Stat attackSpeed = new Stat { statName = "Attack Speed", level = 0, baseValue = 1f, addValue = 0.2f };
    public float currentHealth = 0;

    // references for values and levels to display in UI
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private TextMeshProUGUI armorLevelText;
    [SerializeField] private TextMeshProUGUI movementSpeedLevelText;
    [SerializeField] private TextMeshProUGUI sprintSpeedLevelText;
    [SerializeField] private TextMeshProUGUI staminaLevelText;
    [SerializeField] private TextMeshProUGUI attackDamageLevelText;
    [SerializeField] private TextMeshProUGUI attackRangeLevelText;
    [SerializeField] private TextMeshProUGUI attackSpeedLevelText;

    // reference for health slider
    [SerializeField] private Slider healthSlider;

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
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor.Value, 0);
        currentHealth -= effectiveDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            // TODO: die logic
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

    public void IncreaseLevel(Stat stat, int amount = 1)
    {
        stat.level += amount;
        Debug.Log(stat.statName);
        if (stat.statName == "Health") { UpdateHealthMaxValue(); }
    }

    private void RefreshTotalHealthUI()
    {
        healthSlider.maxValue = maxHealth.Value;
        healthSlider.value = currentHealth;
        healthAmountText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth.Value)}";
    }
}
