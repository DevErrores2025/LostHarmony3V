using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 25f;
    public float speed = 10f;
    public float lifetime = 3f;
    
    [Header("Visual Effects")]
    public GameObject hitEffect;
    public bool destroyOnHit = true;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Rigidbody2D rb;
    private Vector2 direction;
    private bool hasHit = false;
    private bool isInitialized = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Verificar que el Rigidbody2D esté configurado correctamente
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }
        
        // Auto-destruir después del lifetime
        Destroy(gameObject, lifetime);
        
        if (enableDebug) Debug.Log("Player projectile created!");
    }
    
    void Update()
    {
        // Si no se inicializó correctamente, mover manualmente
        if (!isInitialized && rb != null)
        {
            // Movimiento por defecto hacia la derecha
            direction = Vector2.right;
            rb.linearVelocity = direction * speed;
            isInitialized = true;
            
            if (enableDebug) Debug.Log("Projectile moving with default direction!");
        }
        
        // Debug: mostrar velocidad actual
        if (enableDebug && rb != null)
        {
            Debug.Log($"Projectile velocity: {rb.linearVelocity}");
        }
        
        // Opcional: Rotar el proyectil mientras vuela
        transform.Rotate(0, 0, 360f * Time.deltaTime);
    }
    
    public void Initialize(Vector2 shootDirection, float projectileSpeed, float projectileDamage, float projectileLifetime)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        lifetime = projectileLifetime;
        
        if (enableDebug) Debug.Log($"Initializing projectile - Direction: {direction}, Speed: {speed}");
        
        // Aplicar velocidad inmediatamente
        if (rb != null)
        {
            rb.gravityScale = 0f; // Asegurar que no tenga gravedad
            rb.linearVelocity = direction * speed;
            isInitialized = true;
            
            if (enableDebug) Debug.Log($"Projectile velocity set to: {rb.linearVelocity}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        
        if (enableDebug) Debug.Log($"Projectile hit: {other.name} with tag: {other.tag}");
        
        // Verificar si golpeó al boss o enemigos
        if (other.CompareTag("Boss"))
        {
            // Dañar al boss
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                if (enableDebug) Debug.Log($"Projectile dealt {damage} damage to boss!");
            }
            
            Hit();
        }
        // Verificar si golpeó otros enemigos (para expansión futura)
        else if (other.CompareTag("Enemy"))
        {
            // Aquí puedes añadir lógica para otros enemigos
            if (enableDebug) Debug.Log("Hit enemy!");
            Hit();
        }
        // Verificar si golpeó el suelo u obstáculos
        else if (other.CompareTag("Ground"))
        {
            if (enableDebug) Debug.Log("Projectile hit ground/wall/obstacle!");
            Hit();
        }
        // Ignorar al jugador y sus proyectiles
        else if (other.CompareTag("Player") || other.CompareTag("PlayerAttack"))
        {
            return;
        }
    }
    
    void Hit()
    {
        if (hasHit) return;
        
        hasHit = true;
        
        if (enableDebug) Debug.Log("Projectile hit something!");
        
        // Crear efecto de impacto
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Limpiar el efecto después de 2 segundos
        }
        
        // Detener el proyectil
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Destruir el proyectil si está configurado para ello
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
    
    // Método para cambiar el daño dinámicamente
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
    
    // Método para cambiar la velocidad dinámicamente
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }
}