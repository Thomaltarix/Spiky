using UnityEngine;

[System.Serializable]
public class Stat
{
    public string statName;  // The name of the stat (e.g., Health, Armor)
    public int level;        // The current level of the stat
    public float baseValue;  // Base value used for calculations

    // The current value of the stat, dynamically calculated based on level and base value
    public float Value
    {
        get
        {
            return baseValue * level; // Example calculation, can be customized
        }
    }

    // Method to increase the level of the stat
    public void IncreaseLevel(int amount = 1)
    {
        level += amount;
    }
}
