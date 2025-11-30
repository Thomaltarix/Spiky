using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField] private const float _timerCooldown = 2f;
    private float _timer;

    public int EnemyCount { get; private set; } = 0;

    private SphereCollider _sphere;
    private LayerMask obstacleLayer;

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && _sphere != null)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float dist = Mathf.Sqrt(Random.Range(0f, 1f)) * _sphere.radius;

            Vector3 randomPos = transform.position + new Vector3(
                Mathf.Cos(angle) * dist,
                0,
                Mathf.Sin(angle) * dist
            );

            if (!IsPositionBlocked(randomPos))
            {
                Instantiate(enemyPrefab, randomPos, Quaternion.identity);
                EnemyCount++;
            }
            else
            {
                Debug.Log("Position de spawn bloquée par un obstacle.");
            }
        }
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        RaycastHit hit;

        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f, obstacleLayer))
        {
            Debug.DrawRay(position + Vector3.up * 10f, Vector3.down * 20f, Color.red, 2f);
            return true;
        }

        if (Physics.SphereCast(position, 1f, Vector3.up, out hit, 10f, obstacleLayer))
        {
            Debug.DrawRay(position, Vector3.up * 10f, Color.green, 2f);
            return true;
        }

        return false;
    }

    private void Start()
    {
        _sphere = GetComponent<SphereCollider>();
        obstacleLayer = LayerMask.GetMask("Obstacle");
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
