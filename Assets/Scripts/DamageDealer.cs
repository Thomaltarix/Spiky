using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    // add reference to stat manager
    [Header("References")]
    [SerializeField] private PlayerStatManager playerStats;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();

        // make sure reference to stat manager is found
        if (playerStats == null)
        {
            playerStats = GetComponentInParent<PlayerStatManager>();
        }

        if (playerStats == null)
        {
            Debug.LogError("DamageDealer couldn't find PlayerStatManager!");
        }
    }

    void Update()
    {
        if (canDealDamage && playerStats != null)
        {
            RaycastHit hit;
            float currentRange = playerStats.attackRange.Value;

            int layerMask =1 << 9;
            if (Physics.Raycast(transform.position, -transform.up, out hit, currentRange, layerMask))
            {
                GameObject target = hit.transform.gameObject;

                if (!hasDealtDamage.Contains(target))
                {
                    hasDealtDamage.Add(target);

                    Health hp = target.GetComponent<Health>();
                    if (hp != null)
                    {
                        float currentDamage = playerStats.attackDamage.Value;
                        hp.TakeDamage(currentDamage, playerStats);
                    }
                }
            }
        }
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos only work when the game is running and playerStats was found
        // Use fallback value for editor here
        float debugLength = (playerStats != null) ? playerStats.attackRange.Value : 1.0f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, -transform.up * debugLength);
    }
}