using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    // define different stats as public, so they can be visible in Unity Inspector
    public Stat maxHealth = new Stat { statName = "Health", level = 0, baseValue = 20f, addValue = 5f };
    public Stat armor = new Stat { statName = "Armor", level = 0, baseValue = 0f, addValue = 5f };
    public Stat movementSpeed = new Stat { statName = "Movement Speed", level = 0, baseValue = 5f, addValue = 0.5f };
    public Stat sprintSpeed = new Stat { statName = "Sprint Speed", level = 0, baseValue = 8f, addValue = 0.5f };
    public Stat stamina = new Stat { statName = "Stamina", level = 0, baseValue = 10f, addValue = 1f };
    public Stat attackDamage = new Stat { statName = "Attack Damage", level = 0, baseValue = 20f, addValue = 5f };
    public Stat attackRange = new Stat { statName = "Attack Range", level = 0, baseValue = 5f, addValue = 1f };
    public Stat attackSpeed = new Stat { statName = "Attack Speed", level = 0, baseValue = 0.75f, addValue = 5f };
    public float currentHealth = 0;

    // references for values and levels to display in UI
    public Slider healthSlider;
    public TextMeshProUGUI healthAmountText;
    public TextMeshProUGUI armorLevelText;
    public TextMeshProUGUI movementSpeedLevelText;
    public TextMeshProUGUI sprintSpeedLevelText;
    public TextMeshProUGUI staminaLevelText;
    public TextMeshProUGUI attackDamageLevelText;
    public TextMeshProUGUI attackRangeLevelText;
    public TextMeshProUGUI attackSpeedLevelText;

    void Start()
    {

        // intial test values
        maxHealth.level = 0;
        currentHealth = maxHealth.Value;
        armor.level = 0;
        movementSpeed.level = 0;
        sprintSpeed.level = 0;
        stamina.level = 0;
        attackDamage.level = 0;
        attackRange.level = 0; 
        attackSpeed.level = 0;

        // initialize health bar
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth.baseValue * maxHealth.level;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // Refresh texts with current values
        if (armorLevelText != null) { armorLevelText.text = armor.level.ToString(); }
        if (movementSpeedLevelText != null) { movementSpeedLevelText.text = movementSpeed.level.ToString(); }
        if (sprintSpeedLevelText != null) { sprintSpeedLevelText.text = sprintSpeed.level.ToString(); }
        if (staminaLevelText != null) { staminaLevelText.text = stamina.level.ToString(); }
        if (attackDamageLevelText != null) { attackDamageLevelText.text = attackDamage.level.ToString(); }
        if (attackRangeLevelText != null) { attackRangeLevelText.text = attackRange.level.ToString(); }
        if (attackSpeedLevelText != null) { attackSpeedLevelText.text = attackSpeed.level.ToString(); }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth.Value;
            healthSlider.value = Mathf.Min(healthSlider.value, maxHealth.Value);
        }

        if (healthAmountText != null)
            healthAmountText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth.Value)}"; // just an example to start with
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor.Value, 0);
        currentHealth -= effectiveDamage;
        if (currentHealth < 0) currentHealth = 0;
    }

    //call this function when the health level is getting updated
    public void UpdateHealthMaxValue()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth.baseValue * maxHealth.level;
        }

    }
}