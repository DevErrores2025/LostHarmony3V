using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private int direction = 1; // 1 = derecha, -1 = izquierda
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // Mover el enemigo
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con algo que tenga el tag "Wall"
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Cambiar dirección
            direction *= -1;
            
            // Voltear sprite (opcional)
            transform.localScale = new Vector3(-transform.localScale.x, 
                                             transform.localScale.y, 
                                             transform.localScale.z);
        }
    }
}