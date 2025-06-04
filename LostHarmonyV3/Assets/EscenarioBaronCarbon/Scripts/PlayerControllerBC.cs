using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;
    
    [Header("Attack System")]
    public GameObject projectilePrefab;
    public Transform attackPoint;
    public float projectileSpeed = 10f;
    public float attackCooldown = 0.5f;
    public KeyCode attackKey = KeyCode.X;
    
    [Header("Attack Settings")]
    public float projectileLifetime = 3f;
    public float projectileDamage = 25f;
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;
    private bool isGrounded;
    private float lastAttackTime;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Crear ground check automáticamente
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            
            // AJUSTAR POSICIÓN BASÁNDOSE EN EL COLLIDER
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                // Colocar el ground check justo debajo del collider
                float colliderBottom = playerCollider.bounds.min.y - transform.position.y;
                groundCheckObj.transform.localPosition = new Vector3(0, colliderBottom - 0.1f, 0);
            }
            else
            {
                // Posición por defecto si no hay collider
                groundCheckObj.transform.localPosition = new Vector3(0, -0.6f, 0);
            }
            
            groundCheck = groundCheckObj.transform;
        }
        
        // Crear attack point automáticamente
        if (attackPoint == null)
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(transform);
            attackPointObj.transform.localPosition = new Vector3(0.8f, 0.2f, 0);
            attackPoint = attackPointObj.transform;
        }
    }
    
    void Update()
    {
        // CONTROLES SIMPLES
        float horizontal = Input.GetAxis("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool attackPressed = Input.GetKeyDown(attackKey);
        
        // MÚLTIPLES MÉTODOS DE GROUND CHECK
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        // Método alternativo con Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayerMask);
        bool isGroundedRay = hit.collider != null;
        
        // Método alternativo con BoxCast
        Vector2 boxSize = new Vector2(0.8f, 0.1f);
        RaycastHit2D boxHit = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, 0.1f, groundLayerMask);
        bool isGroundedBox = boxHit.collider != null;
        
        // Usar cualquier método que detecte suelo
        bool finalGrounded = isGrounded || isGroundedRay || isGroundedBox;
        
        // DEBUG DETALLADO
        if (enableDebug)
        {
            Debug.Log($"Jump: {jumpPressed} | Circle: {isGrounded} | Ray: {isGroundedRay} | Box: {isGroundedBox} | Final: {finalGrounded}");
            Debug.Log($"GroundCheck Pos: {groundCheck.position} | Player Pos: {transform.position}");
        }
        
        // MOVIMIENTO
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        
        // SALTO CON MÚLTIPLES VERIFICACIONES
        if (jumpPressed)
        {
            if (enableDebug) Debug.Log("Space pressed!");
            if (finalGrounded)
            {
                if (enableDebug) Debug.Log("Jumping!");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else
            {
                if (enableDebug) Debug.Log($"Not grounded - Circle:{isGrounded}, Ray:{isGroundedRay}, Box:{isGroundedBox}");
            }
        }
        
        // SISTEMA DE ATAQUE
        if (attackPressed && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
        
        // ACTUALIZAR ANIMACIONES (usar finalGrounded)
        UpdateAnimations(horizontal, finalGrounded);
        
        // VOLTEAR SPRITE
        if (horizontal > 0 && !facingRight)
            Flip();
        else if (horizontal < 0 && facingRight)
            Flip();
    }
    
    void Attack()
    {
        if (projectilePrefab == null)
        {
            if (enableDebug) Debug.LogWarning("No projectile prefab assigned!");
            return;
        }
        
        // Determinar dirección del ataque
        Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;
        
        // Crear proyectil
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        
        // Configurar el proyectil
        PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(attackDirection, projectileSpeed, projectileDamage, projectileLifetime);
        }
        else
        {
            // Si no tiene el script, aplicar velocidad directamente
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = attackDirection * projectileSpeed;
            }
            
            // Autodestruir después del lifetime
            Destroy(projectile, projectileLifetime);
        }
        
        // Rotar proyectil para que apunte en la dirección correcta
        if (!facingRight)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x *= -1;
            projectile.transform.localScale = scale;
        }
        
        if (enableDebug) Debug.Log($"Player attacked! Direction: {attackDirection}");
    }
    
    void UpdateAnimations(float horizontal, bool grounded)
    {
        if (animator != null)
        {
            // Pasar la velocidad al animator (valor absoluto para que funcione en ambas direcciones)
            float speed = Mathf.Abs(horizontal);
            animator.SetFloat("Speed", speed);
            
            // Pasar si está en el suelo (útil para animaciones de salto)
            animator.SetBool("IsGrounded", grounded);
            
            // Trigger de ataque (opcional)
            if (Input.GetKeyDown(attackKey) && Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        
        // VOLTEAR TODO EL PERSONAJE COMPLETO
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, 0.2f);
            
            // Mostrar dirección de ataque
            Vector3 attackDir = facingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(attackPoint.position, attackDir * 2f);
        }
    }
}