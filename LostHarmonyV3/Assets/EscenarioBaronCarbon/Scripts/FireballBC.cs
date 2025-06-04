using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float damage = 20f;
    public float lifeTime = 5f;
    public float speed = 8f;
    
    [Header("Visual Effects")]
    public GameObject explosionEffect;
    public float explosionRadius = 1f;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Rigidbody2D rb;
    private bool hasExploded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Self-destruct after lifetime
        Destroy(gameObject, lifeTime);
        
        if (enableDebug) Debug.Log("Fireball created!");
    }
    
    void Update()
    {
        // Optional: Add some visual trail or rotation here
        // For example, spinning fireball:
        transform.Rotate(0, 0, 360f * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        
        if (enableDebug) Debug.Log($"Fireball hit: {other.name}");
        
        // Check if hit player
        if (other.CompareTag("Player"))
        {
            // Deal damage to player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                if (enableDebug) Debug.Log($"Fireball dealt {damage} damage to player!");
            }
            
            Explode();
        }
        // Check if hit ground or obstacles
        else if (other.CompareTag("Ground"))
        {
            Explode();
        }
        // Ignore boss and other enemy projectiles
        else if (other.CompareTag("Boss") || other.CompareTag("EnemyAttack"))
        {
            return;
        }
    }
    
    void Explode()
    {
        if (hasExploded) return;
        
        hasExploded = true;
        
        if (enableDebug) Debug.Log("Fireball exploded!");
        
        // Create explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f); // Clean up explosion effect
        }
        
        // Optional: Area damage around explosion
        DealAreaDamage();
        
        // Destroy the fireball
        Destroy(gameObject);
    }
    
    void DealAreaDamage()
    {
        // Find all colliders in explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Only deal damage if player is within explosion radius and wasn't hit directly
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    float explosionDamage = damage * 0.5f; // Reduced damage for area effect
                    playerHealth.TakeDamage(explosionDamage);
                    
                    if (enableDebug) Debug.Log($"Explosion dealt {explosionDamage} area damage to player!");
                }
            }
        }
    }
    
    // Optional: Method to set fireball direction and speed from boss
    public void SetDirection(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
    }
    
    // Optional: Method to set custom damage
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}