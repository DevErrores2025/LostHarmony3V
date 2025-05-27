using UnityEngine;

public class BoxGravityInverter : MonoBehaviour
{
    private Rigidbody2D rb;
    private float defaultGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GravityArea"))
        {
            rb.gravityScale = -Mathf.Abs(defaultGravityScale);  // Gravedad invertida
            transform.rotation = Quaternion.Euler(0, 0, 180);   // Rotar la caja
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GravityArea"))
        {
            rb.gravityScale = defaultGravityScale;              // Restaurar gravedad normal
            transform.rotation = Quaternion.Euler(0, 0, 0);     // Restaurar rotación
        }
    }
}
