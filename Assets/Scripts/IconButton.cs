using UnityEngine;
public class IconButton : MonoBehaviour
{
    public PlayerStatManager playerStats;
    public string statName;

    public void OnIconClick()
    {
        playerStats.IncreaseLevel(statName);
    }
}