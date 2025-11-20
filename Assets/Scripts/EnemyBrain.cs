using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    private Transform player;

    [SerializeField]
    public float speed = 3f;

    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
        }

    }
}
