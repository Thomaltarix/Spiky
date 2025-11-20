using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    public WaveManager waveManager;

    private TMP_Text _textDisplay;

    void Awake()
    {
        _textDisplay = GetComponent<TMP_Text>();
    }

    void Update()
    {
        _textDisplay.text = "Wave : " + waveManager.CurrentWave;
    }
}
