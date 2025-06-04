using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public float attackRange = 8f;
    
    [Header("Fireball Attack")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballCooldown = 2.5f;
    public float fireballSpeed = 5f; // Velocidad más lenta
    
    [Header("Fireball Settings")]
    public bool useSpreadShot = false; // Cambiado a false
    public int spreadCount = 1; // Solo 1 bola
    public float spreadAngle = 25f;
    public float randomRangeX = 11f; // Rango de 1 a -10 (11 unidades total)
    public float randomRangeY = 12f; // Rango de 6 a -6 (12 unidades total)
    
    [Header("Toxic Smoke Attack")]
    public GameObject toxicSmokePrefab;
    public float smokeCooldown = 6f;
    public int smokeCount = 3;
    public float smokeSpacing = 2f;
    
    [Header("Smoke Area Settings")]
    public float smokeRangeX = 12f;
    public float smokeRangeY = 3f;
    public LayerMask groundLayerMask = 1;
    public float groundCheckDistance = 10f;
    
    [Header("Scene Transition")]
    [Tooltip("Nombre de la siguiente escena cuando el boss muera")]
    public string nextSceneName = "Alcantarillas";
    [Tooltip("Tiempo de espera antes de cambiar de escena")]
    public float sceneChangeDelay = 3f;
    [Tooltip("Mostrar mensaje de victoria")]
    public bool showVictoryMessage = true;
    
    [Header("Victory UI (Opcional)")]
    public GameObject victoryPanel;
    public UnityEngine.UI.Text victoryText;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private bool isDead = false;
    
    // Attack timers
    private float lastFireballTime;
    private float lastSmokeTime;
    
    // States
    private enum BossState { Idle, Chasing, Attacking, Dead }
    private BossState currentState = BossState.Idle;
    
    void Start()
    {
        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            if (enableDebug) Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }
        
        // Create spawn points if they don't exist
        CreateSpawnPoints();
        
        // Hide victory panel at start
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }
    
    void CreateSpawnPoints()
    {
        if (fireballSpawnPoint == null)
        {
            GameObject fireballSpawn = new GameObject("FireballSpawnPoint");
            fireballSpawn.transform.SetParent(transform);
            fireballSpawn.transform.localPosition = new Vector3(1f, 0.5f, 0);
            fireballSpawnPoint = fireballSpawn.transform;
        }
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // State machine
        switch (currentState)
        {
            case BossState.Idle:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = BossState.Chasing;
                    if (enableDebug) Debug.Log("Boss detected player!");
                }
                break;
                
            case BossState.Chasing:
                ChasePlayer();
                
                if (distanceToPlayer <= attackRange)
                {
                    currentState = BossState.Attacking;
                    if (enableDebug) Debug.Log("Boss entering attack mode!");
                }
                else if (distanceToPlayer > detectionRange * 1.5f)
                {
                    currentState = BossState.Idle;
                }
                break;
                
            case BossState.Attacking:
                AttackPlayer();
                
                if (distanceToPlayer > attackRange * 1.2f)
                {
                    currentState = BossState.Chasing;
                }
                break;
        }
        
        // Always face the player when detected
        if (distanceToPlayer <= detectionRange)
        {
            FacePlayer();
        }
    }
    
    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        
        if (enableDebug) Debug.Log($"Chasing player. Distance: {Vector2.Distance(transform.position, player.position):F1}");
    }
    
    void AttackPlayer()
    {
        // Stop moving while attacking
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        // Try fireball attack
        if (Time.time >= lastFireballTime + fireballCooldown)
        {
            FireRandomPositionShot();
            lastFireballTime = Time.time;
        }
        
        // Try smoke attack
        if (Time.time >= lastSmokeTime + smokeCooldown)
        {
            StartCoroutine(ToxicSmokeAttackRandomPositions());
            lastSmokeTime = Time.time;
        }
    }
    
    void FireRandomPositionShot()
    {
        if (fireballPrefab == null || player == null) return;
        
        // Generar posición aleatoria en el rango especificado
        Vector3 randomTargetPosition = new Vector3(
            player.position.x + Random.Range(-10f, 1f), // De -10 a 1
            player.position.y + Random.Range(-6f, 6f),  // De -6 a 6
            0
        );
        
        // Calcular dirección desde el spawn point hacia la posición aleatoria
        Vector2 direction = (randomTargetPosition - fireballSpawnPoint.position).normalized;
        
        // Crear la bola de fuego
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        
        // Aplicar velocidad
        Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
        if (fireballRb != null)
        {
            fireballRb.linearVelocity = direction * fireballSpeed;
        }
        
        // Rotar la bola de fuego para que apunte en la dirección correcta
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        fireball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        if (enableDebug) 
        {
            Debug.Log($"Boss fired fireball towards random position: {randomTargetPosition}");
            Debug.DrawLine(fireballSpawnPoint.position, randomTargetPosition, Color.red, 2f);
        }
    }
    
    IEnumerator ToxicSmokeAttackRandomPositions()
    {
        if (toxicSmokePrefab == null || player == null) yield break;
        
        if (enableDebug) Debug.Log("Boss creating toxic smoke in random positions!");
        
        // Create multiple smoke clouds in random positions around the map
        for (int i = 0; i < smokeCount; i++)
        {
            Vector3 smokePosition = GetRandomGroundPosition();
            
            if (smokePosition != Vector3.zero)
            {
                GameObject smoke = Instantiate(toxicSmokePrefab, smokePosition, Quaternion.identity);
                
                if (enableDebug) Debug.Log($"Smoke {i + 1} created at: {smokePosition}");
            }
            
            // Small delay between each smoke spawn
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    Vector3 GetRandomGroundPosition()
    {
        // Try to find a random position on the ground
        for (int attempts = 0; attempts < 10; attempts++)
        {
            // Generate random position around the player area
            Vector3 randomPosition = new Vector3(
                player.position.x + Random.Range(-smokeRangeX, smokeRangeX),
                player.position.y + smokeRangeY, // Start above ground
                0
            );
            
            // Raycast down to find ground
            RaycastHit2D hit = Physics2D.Raycast(randomPosition, Vector2.down, groundCheckDistance, groundLayerMask);
            
            if (hit.collider != null)
            {
                // Found ground, place smoke slightly above it
                Vector3 groundPosition = hit.point;
                groundPosition.y += 0.5f; // Slightly above ground
                
                if (enableDebug) 
                {
                    Debug.DrawRay(randomPosition, Vector2.down * groundCheckDistance, Color.green, 2f);
                }
                
                return groundPosition;
            }
            else
            {
                if (enableDebug) Debug.DrawRay(randomPosition, Vector2.down * groundCheckDistance, Color.red, 2f);
            }
        }
        
        // Fallback: use player's ground level if no ground found
        if (enableDebug) Debug.Log("No ground found, using fallback position");
        return new Vector3(
            player.position.x + Random.Range(-smokeRangeX/2, smokeRangeX/2),
            player.position.y,
            0
        );
    }
    
    void FacePlayer()
    {
        if (player == null) return;
        
        bool shouldFaceRight = player.position.x > transform.position.x;
        
        if (shouldFaceRight && !facingRight)
        {
            Flip();
        }
        else if (!shouldFaceRight && facingRight)
        {
            Flip();
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (enableDebug) Debug.Log($"Boss took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        // Flash effect (optional)
        StartCoroutine(DamageFlash());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    IEnumerator DamageFlash()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        currentState = BossState.Dead;
        
        if (enableDebug) Debug.Log("¡Boss derrotado! Cambiando a la escena de Alcantarillas...");
        
        // Stop all movement
        rb.linearVelocity = Vector2.zero;
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Start death sequence
        StartCoroutine(DeathSequence());
    }
    
    IEnumerator DeathSequence()
    {
        // Show victory message/panel if enabled
        if (showVictoryMessage)
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                if (victoryText != null)
                {
                    victoryText.text = "¡Boss Derrotado!\nAvanzando al siguiente nivel...";
                }
            }
            else
            {
                if (enableDebug) Debug.Log("¡VICTORIA! Boss derrotado. Siguiente nivel en " + sceneChangeDelay + " segundos...");
            }
        }
        
        // Wait for the specified delay
        yield return new WaitForSeconds(sceneChangeDelay);
        
        // Change to next scene
        ChangeToNextScene();
    }
    
    void ChangeToNextScene()
    {
        if (enableDebug) Debug.Log($"Cambiando a la escena: {nextSceneName}");
        
        // Verify the scene exists before trying to load it
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"¡Escena '{nextSceneName}' no encontrada! Verifica que esté en Build Settings.");
            
            // Fallback: try loading by build index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                if (enableDebug) Debug.Log($"Usando fallback: cargando escena índice {nextSceneIndex}");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogError("No hay más escenas disponibles. Verifica tu Build Settings.");
            }
        }
    }
    
    // Método público para cambiar la escena manualmente (útil para botones)
    public void ForceChangeToNextScene()
    {
        if (enableDebug) Debug.Log("Forzando cambio de escena...");
        ChangeToNextScene();
    }
    
    // Método para recibir daño de proyectiles del jugador
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other.CompareTag("PlayerAttack"))
        {
            PlayerProjectile projectile = other.GetComponent<PlayerProjectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
                if (enableDebug) Debug.Log($"Boss hit by player projectile for {projectile.damage} damage!");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw fireball spawn point
        if (fireballSpawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(fireballSpawnPoint.position, 0.2f);
        }
        
        // Draw smoke area range (around player if exists)
        if (player != null)
        {
            Gizmos.color = Color.green;
            Vector3 smokeCenter = player.position;
            Gizmos.DrawWireCube(smokeCenter, new Vector3(smokeRangeX * 2, smokeRangeY * 2, 0));
        }
    }
}