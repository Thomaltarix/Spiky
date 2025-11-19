using TMPro;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    // Define the different stats as public, so they can be visible in Unity Inspector
    public Stat health = new Stat { statName = "Health", level = 5, baseValue = 20f };
    public Stat armor = new Stat { statName = "Armor", level = 3, baseValue = 5f };
    public Stat movementSpeed = new Stat { statName = "Movement Speed", level = 4, baseValue = 1.5f };
    public Stat sprintSpeed = new Stat { statName = "Sprint Speed", level = 2, baseValue = 2.5f };
    public Stat stamina = new Stat { statName = "Stamina", level = 6, baseValue = 10f };
    public Stat attackDamage = new Stat { statName = "Attack Damage", level = 4, baseValue = 7f };
    public Stat attackRange = new Stat { statName = "Attack Range", level = 1, baseValue = 1.2f };
    public Stat attackSpeed = new Stat { statName = "Attack Speed", level = 3, baseValue = 0.75f };

    // Text references for labels and values
    public TextMeshProUGUI healthValueText;
    public TextMeshProUGUI healthLabelText;

    public TextMeshProUGUI armorValueText;
    public TextMeshProUGUI armorLabelText;

    public TextMeshProUGUI movementSpeedValueText;
    public TextMeshProUGUI movementSpeedLabelText;

    public TextMeshProUGUI sprintSpeedValueText;
    public TextMeshProUGUI sprintSpeedLabelText;

    public TextMeshProUGUI staminaValueText;
    public TextMeshProUGUI staminaLabelText;

    public TextMeshProUGUI attackDamageValueText;
    public TextMeshProUGUI attackDamageLabelText;

    public TextMeshProUGUI attackRangeValueText;
    public TextMeshProUGUI attackRangeLabelText;

    public TextMeshProUGUI attackSpeedValueText;
    public TextMeshProUGUI attackSpeedLabelText;

    void Start()
    {
        Debug.Log("Player stats initialized.");

        // initialize labels for stats
        if (healthLabelText != null)
            healthLabelText.text = health.statName;

        if (armorLabelText != null)
            armorLabelText.text = armor.statName;

        if (movementSpeedLabelText != null)
            movementSpeedLabelText.text = movementSpeed.statName;

        if (sprintSpeedLabelText != null)
            sprintSpeedLabelText.text = sprintSpeed.statName;

        if (staminaLabelText != null)
            staminaLabelText.text = stamina.statName;

        if (attackDamageLabelText != null)
            attackDamageLabelText.text = attackDamage.statName;

        if (attackRangeLabelText != null)
            attackRangeLabelText.text = attackRange.statName;

        if (attackSpeedLabelText != null)
            attackSpeedLabelText.text = attackSpeed.statName;

        // Example: Log current valueS
        Debug.Log($"Health: {health.CurrentValue}");
        Debug.Log($"Armor: {armor.CurrentValue}");
        Debug.Log($"Movement Speed: {movementSpeed.CurrentValue}");
        Debug.Log($"Sprint Speed: {sprintSpeed.CurrentValue}");
        Debug.Log($"Stamina: {stamina.CurrentValue}");
        Debug.Log($"Attack Damage: {attackDamage.CurrentValue}");
        Debug.Log($"Attack Range: {attackRange.CurrentValue}");
        Debug.Log($"Attack Speed: {attackSpeed.CurrentValue}");
    }

    void Update()
    {
        // Refresh texts with current values
        if (healthValueText != null) { healthValueText.text = health.CurrentValue.ToString(); }
        if (armorValueText != null) { armorValueText.text = health.CurrentValue.ToString(); }
        if (movementSpeedValueText != null) { movementSpeedValueText.text = health.CurrentValue.ToString(); }
        if (sprintSpeedValueText != null) { sprintSpeedValueText.text = health.CurrentValue.ToString(); }
        if (staminaValueText != null) { staminaValueText.text = health.CurrentValue.ToString(); }
        if (attackDamageValueText != null) { attackDamageValueText.text = health.CurrentValue.ToString(); }
        if (attackRangeValueText != null) { attackRangeValueText.text = health.CurrentValue.ToString(); }
        if (attackSpeedValueText != null) { attackSpeedValueText.text = health.CurrentValue.ToString(); }
    }

    // Example function
    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor.CurrentValue, 0);
        health.baseValue -= effectiveDamage;
        Debug.Log($"Took damage: {effectiveDamage}, Health left: {health.baseValue}");
    }
}
