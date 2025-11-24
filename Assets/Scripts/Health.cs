using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _health = 100f;
    [SerializeField] private float ragdollDuration = 2f;
    [SerializeField] private GameObject smokePrefab; // assigner un prefab de particules de fumée

    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Renderer[] renderers;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        DisableRagdoll();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        _health -= damage;

        if (_health <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
            animator.enabled = false;

        EnableRagdoll();

        MakeWhite();

        DeadParticles();

        Invoke(nameof(DestroySelf), ragdollDuration);
    }


    private void DeadParticles() 
    {
        if (smokePrefab != null)
        {
            GameObject smoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = smoke.GetComponent<ParticleSystem>();
            Destroy(smoke, 2f);
           
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void EnableRagdoll()
    {
        foreach (var col in ragdollColliders)
        {
            if (col.gameObject != this.gameObject)
                col.enabled = true;
            else
                col.enabled = false;
        }

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.mass = 1f;
        }
    }

    private void DisableRagdoll()
    {
        foreach (var rb in ragdollBodies)
            rb.isKinematic = true;

        foreach (var col in ragdollColliders)
        {
            if (col.gameObject != this.gameObject)
                col.enabled = false;
        }
    }

    private void MakeWhite()
    {
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                rend.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                rend.material.color = Color.black;
            }
        }
    }
}
