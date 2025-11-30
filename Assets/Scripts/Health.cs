using System.Collections;
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

    [SerializeField] private Color hitColor = Color.red;
    private Color originalColor = Color.white;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        DisableRagdoll();
    }


    public void HitFlash()
    {
        StartCoroutine(FlashRoutine(2));
    }

    private IEnumerator FlashRoutine(int flashes)
    {
        for (int i = 0; i < flashes; i++)
        {
            MakeInColor(hitColor);
            yield return new WaitForSeconds(0.1f);

            MakeInColor(originalColor);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        _health -= damage;

        if (_health <= 0f)
            Die();
        else
            HitFlash();
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
            animator.enabled = false;

        EnableRagdoll();

        MakeInColor(Color.black);

        Invoke(nameof(DeadParticles), ragdollDuration);
        Invoke(nameof(DestroySelf), ragdollDuration);
    }


    private void DeadParticles() 
    {
        if (smokePrefab != null)
        {
            GameObject smoke = Instantiate(smokePrefab, transform.position, smokePrefab.transform.rotation);
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

    private void MakeInColor(Color color)
    {
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                //rend.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                rend.material.color = color;
            }
        }
    }
}
