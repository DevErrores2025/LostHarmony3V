using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI References")]
    public Slider healthBar;
    public Text healthText;
    
    [Header("Damage Settings")]
    public float invulnerabilityTime = 1f;
    public Color damageColor = Color.red;
    
    [Header("Death Settings")]
    public string gameOverScene = "GameOver";
    public bool respawnOnDeath = true;
    public Vector3 respawnPosition;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private bool isInvulnerable = false;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Events
    public System.Action<float> OnHealthChanged;
    public System.Action OnPlayerDeath;
    
    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Set respawn position to current position if not set
        if (respawnPosition == Vector3.zero)
        {
            respawnPosition = transform.position;
        }
        
        // Update UI
        UpdateHealthUI();
        
        if (enableDebug) Debug.Log($"Player health initialized: {currentHealth}/{maxHealth}");
    }
    
    void Update()
    {
        // Debug input for testing
        if (enableDebug)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamage(10f); // Test damage
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Heal(20f); // Test healing
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;
        
        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (enableDebug) Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        
        // Update UI
        UpdateHealthUI();
        
        // Visual feedback
        StartCoroutine(DamageEffect());
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Start invulnerability period
            StartCoroutine(InvulnerabilityPeriod());
        }
    }
    
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        float oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        float actualHeal = currentHealth - oldHealth;
        
        if (enableDebug && actualHeal > 0) 
            Debug.Log($"Player healed for {actualHeal}! Health: {currentHealth}/{maxHealth}");
        
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        
        // Update UI
        UpdateHealthUI();
        
        // Visual feedback for healing
        StartCoroutine(HealEffect());
    }
    
    public void SetMaxHealth(float newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        if (enableDebug) Debug.Log("Player died!");
        
        // Trigger death event
        OnPlayerDeath?.Invoke();
        
        // Disable player controller
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Death visual effect
        StartCoroutine(DeathEffect());
        
        // Handle respawn or game over
        if (respawnOnDeath)
        {
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            // Load game over scene or show game over UI
            StartCoroutine(GameOver());
        }
    }
    
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f);
        
        // Reset position
        transform.position = respawnPosition;
        
        // Reset health
        currentHealth = maxHealth;
        isDead = false;
        
        // Re-enable player controller
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Reset sprite color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // Update UI
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        
        // Brief invulnerability after respawn
        StartCoroutine(InvulnerabilityPeriod());
        
        if (enableDebug) Debug.Log("Player respawned!");
    }
    
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        
        // You can implement scene loading here
        // UnityEngine.SceneManagement.SceneManager.LoadScene(gameOverScene);
        
        if (enableDebug) Debug.Log("Game Over!");
    }
    
    IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;
        
        // Flashing effect during invulnerability
        if (spriteRenderer != null)
        {
            for (float i = 0; i < invulnerabilityTime; i += 0.1f)
            {
                spriteRenderer.color = Color.clear;
                yield return new WaitForSeconds(0.05f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.05f);
            }
            
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityTime);
        }
        
        isInvulnerable = false;
    }
    
    IEnumerator DamageEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    
    IEnumerator HealEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    
    IEnumerator DeathEffect()
    {
        if (spriteRenderer != null)
        {
            // Fade out effect
            Color fadeColor = originalColor;
            for (float i = 1f; i >= 0; i -= 0.05f)
            {
                fadeColor.a = i;
                spriteRenderer.color = fadeColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
    
    void UpdateHealthUI()
    {
        // Update health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
        }
    }
    
    // Collision detection for damage sources
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        // Check for damage sources
        if (other.CompareTag("EnemyAttack") || other.CompareTag("Fireball"))
        {
            TakeDamage(20f);
        }
        else if (other.CompareTag("ToxicSmoke"))
        {
            TakeDamage(15f);
        }
        else if (other.CompareTag("HealthPickup"))
        {
            Heal(25f);
            Destroy(other.gameObject);
        }
    }
    
    // Public getters for other scripts
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsAlive()
    {
        return !isDead;
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
}