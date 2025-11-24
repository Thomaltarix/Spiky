using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private EnemySpawner _spawner;


    private int _enemyWaveNumber = 5;
    public int CurrentWave { get; private set; } = 1;

    public void NextWave()
    {
        _enemyWaveNumber += 5;
        CurrentWave++;
    }

    private void Update()
    {
        int currentEnemyCount = _spawner.EnemyCount;

        if (currentEnemyCount >= _enemyWaveNumber)
        {
            NextWave();
        }
    }
}
