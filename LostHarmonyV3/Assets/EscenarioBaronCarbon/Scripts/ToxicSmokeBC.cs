using UnityEngine;
using System.Collections;

public class ToxicSmoke : MonoBehaviour
{
    [Header("Smoke Settings")]
    public float damage = 15f;
    public float damageInterval = 1f; // Damage every second
    public float duration = 8f; // How long the smoke lasts
    public float fadeInTime = 1f;
    public float fadeOutTime = 2f;
    
    [Header("Visual Settings")]
    public float maxAlpha = 0.7f;
    public float pulseSpeed = 2f;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D smokeCollider;
    private bool isActive = false;
    private bool isDissipating = false;
    
    // Keep track of player in smoke
    private bool playerInSmoke = false;
    private Coroutine damageCoroutine;
    
    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        smokeCollider = GetComponent<CircleCollider2D>();
        
        // Make sure it's a trigger
        if (smokeCollider != null)
        {
            smokeCollider.isTrigger = true;
        }
        
        // Start the smoke lifecycle
        StartCoroutine(SmokeLifecycle());
        
        if (enableDebug) Debug.Log("Toxic smoke created!");
    }
    
    void Update()
    {
        if (isActive && !isDissipating)
        {
            // Pulsing effect while active
            PulseEffect();
        }
    }
    
    void PulseEffect()
    {
        if (spriteRenderer == null) return;
        
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.1f + 0.9f;
        Color color = spriteRenderer.color;
        color.a = maxAlpha * pulse;
        spriteRenderer.color = color;
    }
    
    IEnumerator SmokeLifecycle()
    {
        // Phase 1: Fade in
        yield return StartCoroutine(FadeIn());
        
        // Phase 2: Active period
        isActive = true;
        yield return new WaitForSeconds(duration - fadeInTime - fadeOutTime);
        
        // Phase 3: Fade out
        isDissipating = true;
        yield return StartCoroutine(FadeOut());
        
        // Clean up
        if (enableDebug) Debug.Log("Toxic smoke dissipated");
        Destroy(gameObject);
    }
    
    IEnumerator FadeIn()
    {
        if (spriteRenderer == null) yield break;
        
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, elapsedTime / fadeInTime);
            
            color.a = alpha;
            spriteRenderer.color = color;
            
            yield return null;
        }
        
        color.a = maxAlpha;
        spriteRenderer.color = color;
    }
    
    IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;
        
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutTime);
            
            color.a = alpha;
            spriteRenderer.color = color;
            
            yield return null;
        }
        
        color.a = 0f;
        spriteRenderer.color = color;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || isDissipating) return;
        
        if (other.CompareTag("Player"))
        {
            if (enableDebug) Debug.Log("Player entered toxic smoke!");
            
            playerInSmoke = true;
            
            // Start continuous damage
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
            damageCoroutine = StartCoroutine(ContinuousDamage(other));
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enableDebug) Debug.Log("Player left toxic smoke!");
            
            playerInSmoke = false;
            
            // Stop continuous damage
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }
    
    IEnumerator ContinuousDamage(Collider2D player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        while (playerInSmoke && isActive && !isDissipating && playerHealth != null)
        {
            // Deal damage
            playerHealth.TakeDamage(damage);
            
            if (enableDebug) Debug.Log($"Toxic smoke dealt {damage} damage to player!");
            
            // Wait for next damage tick
            yield return new WaitForSeconds(damageInterval);
        }
    }
    
    // Method to set custom damage from boss
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    
    // Method to set custom duration
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw smoke area in editor
        Gizmos.color = Color.green;
        if (smokeCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, smokeCollider.radius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}