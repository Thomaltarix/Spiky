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
    public TextMeshProUGUI healthAmountText;
    public TextMeshProUGUI armorLevelText;
    public TextMeshProUGUI movementSpeedLevelText;
    public TextMeshProUGUI sprintSpeedLevelText;
    public TextMeshProUGUI staminaLevelText;
    public TextMeshProUGUI attackDamageLevelText;
    public TextMeshProUGUI attackRangeLevelText;
    public TextMeshProUGUI attackSpeedLevelText;

    // reference for health slider as it's a special extra component
    public Slider healthSlider;

    void Start()
    {

        // initialize health bar
        currentHealth = maxHealth.Value;

        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth.Value;
        healthSlider.value = currentHealth;

        // initialize UI labels
        armorLevelText.text = armor.level.ToString();
        movementSpeedLevelText.text = movementSpeed.level.ToString();
        sprintSpeedLevelText.text = sprintSpeed.level.ToString();
        staminaLevelText.text = stamina.level.ToString();
        attackDamageLevelText.text = attackDamage.level.ToString();
        attackRangeLevelText.text = attackRange.level.ToString();
        attackSpeedLevelText.text = attackSpeed.level.ToString();

        RefreshHealthLabel();
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
        RefreshHealthLabel();
    }

    //call this function when the health level is getting updated
    public void UpdateHealthMaxValue()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth.baseValue * maxHealth.level;
        }

    }

    public void UpdateLevel(Stat stat)
    {
        stat.level += 1;
    }

    public void RefreshHealthLabel()
    {
        healthAmountText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth.Value)}";
    }
}