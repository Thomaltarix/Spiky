using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    // define different stats as public, so they can be visible in Unity Inspector
    public Stat health = new Stat { statName = "Health", level = 1, baseValue = 20f };
    public Stat armor = new Stat { statName = "Armor", level = 1, baseValue = 5f };
    public Stat movementSpeed = new Stat { statName = "Movement Speed", level = 1, baseValue = 1.5f };
    public Stat sprintSpeed = new Stat { statName = "Sprint Speed", level = 1, baseValue = 2.5f };
    public Stat stamina = new Stat { statName = "Stamina", level = 1, baseValue = 10f };
    public Stat attackDamage = new Stat { statName = "Attack Damage", level = 1, baseValue = 7f };
    public Stat attackRange = new Stat { statName = "Attack Range", level = 1, baseValue = 1.2f };
    public Stat attackSpeed = new Stat { statName = "Attack Speed", level = 1, baseValue = 0.75f };

    // references for values and levels to display in UI
    public Slider healthSlider;
    public TextMeshProUGUI healthLevelText;
    public TextMeshProUGUI armorLevelText;
    public TextMeshProUGUI movementSpeedLevelText;
    public TextMeshProUGUI sprintSpeedLevelText;
    public TextMeshProUGUI staminaLevelText;
    public TextMeshProUGUI attackDamageLevelText;
    public TextMeshProUGUI attackRangeLevelText;
    public TextMeshProUGUI attackSpeedLevelText;

    void Start()
    {
        Debug.Log("Player stats initialized.");

        // initialize health bar
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = health.baseValue * health.level;
            healthSlider.value = healthSlider.maxValue;
        }
    }

    void Update()
    {
        // Refresh texts with current values
        if (healthLevelText != null) { healthLevelText.text = health.level.ToString(); }
        if (armorLevelText != null) { armorLevelText.text = armor.level.ToString(); }
        if (movementSpeedLevelText != null) { movementSpeedLevelText.text = movementSpeed.level.ToString(); }
        if (sprintSpeedLevelText != null) { sprintSpeedLevelText.text = sprintSpeed.level.ToString(); }
        if (staminaLevelText != null) { staminaLevelText.text = stamina.level.ToString(); }
        if (attackDamageLevelText != null) { attackDamageLevelText.text = attackDamage.level.ToString(); }
        if (attackRangeLevelText != null) { attackRangeLevelText.text = attackRange.level.ToString(); }
        if (attackSpeedLevelText != null) { attackSpeedLevelText.text = attackSpeed.level.ToString(); }

        if (healthSlider != null)
            healthSlider.value = health.baseValue;

        if (healthLevelText != null)
            healthLevelText.text = $"{Mathf.CeilToInt(health.baseValue)} / {Mathf.CeilToInt(health.baseValue * health.level)}"; // just an example to start with
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor.CurrentValue, 0);
        health.baseValue -= effectiveDamage;
        if (health.baseValue < 0) health.baseValue = 0;
    }

    // call this function when the health level is getting updated
    public void UpdateHealthMaxValue()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = health.baseValue * health.level;
        }

    }
}