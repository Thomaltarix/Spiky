using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    private const float _timerCooldown = 2f;
    private float _timer;

    public int EnemyCount { get; private set; } = 0;


    private SphereCollider _sphere;

    private void SpawnEnemy()
    {
        if (enemyPrefab != null) {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float dist = Mathf.Sqrt(Random.Range(0f, 1f)) * _sphere.radius;

            Vector3 randomPos = transform.position + new Vector3(
               
                Mathf.Cos(angle) * dist,
                0,
                Mathf.Sin(angle) * dist
            );

            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            EnemyCount++;
        }
    }

    private void Start()
    {
        _sphere = GetComponent<SphereCollider>();

    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _timerCooldown)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }
}
