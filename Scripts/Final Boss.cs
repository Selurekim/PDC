using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float hitInvulnerabilityTime = 1f;

    private int currentHealth;
    private bool isInvulnerable;
    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeHit()
    {
        if (isInvulnerable) return;

        currentHealth--;
        
        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Play hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Start invulnerability
        isInvulnerable = true;
        Invoke(nameof(ResetInvulnerability), hitInvulnerabilityTime);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
            
            // Find and trigger player win
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.Win();
            }
        }
    }

    private void ResetInvulnerability()
    {
        isInvulnerable = false;
    }

    // Override the base Die method to ensure proper cleanup
    public new void Die()
    {
        base.Die();
        
        // Additional boss-specific death effects can be added here
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }
} 