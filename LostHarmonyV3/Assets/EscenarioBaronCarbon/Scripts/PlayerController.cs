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
    
    [Header("Debug")]
    public bool enableDebug = true;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;
    private bool isGrounded;
    
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
    }
    
    void Update()
    {
        // CONTROLES SIMPLES
        float horizontal = Input.GetAxis("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        
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
        Debug.Log($"Jump: {jumpPressed} | Circle: {isGrounded} | Ray: {isGroundedRay} | Box: {isGroundedBox} | Final: {finalGrounded}");
        Debug.Log($"GroundCheck Pos: {groundCheck.position} | Player Pos: {transform.position}");
        
        // MOVIMIENTO
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        
        // SALTO CON MÚLTIPLES VERIFICACIONES
        if (jumpPressed)
        {
            Debug.Log("Space pressed!");
            if (finalGrounded)
            {
                Debug.Log("Jumping!");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else
            {
                Debug.Log($"Not grounded - Circle:{isGrounded}, Ray:{isGroundedRay}, Box:{isGroundedBox}");
            }
        }
        
        // ACTUALIZAR ANIMACIONES (usar finalGrounded)
        UpdateAnimations(horizontal, finalGrounded);
        
        // VOLTEAR SPRITE
        if (horizontal > 0 && !facingRight)
            Flip();
        else if (horizontal < 0 && facingRight)
            Flip();
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
    }
}