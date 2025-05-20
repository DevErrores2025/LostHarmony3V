using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 12f;
    public Transform verificadorSuelo;
    public LayerMask capaSuelo;
    public Animator animator;
    public Rigidbody2D rb;

    private bool enSuelo = false;
    private float direccionMovimiento;
    private bool mirandoDerecha = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Movimiento horizontal
        direccionMovimiento = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(direccionMovimiento * velocidadMovimiento, rb.linearVelocity.y);

        // Saltar
        enSuelo = Physics2D.OverlapCircle(verificadorSuelo.position, 0.1f, capaSuelo);
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Animaciones
        if (enSuelo)
        {
            if (direccionMovimiento != 0)
            {
                animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));

            }
            else
            {
                animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));

            }
        }
        else
        {
            animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));
            // Se puede cambiar por una animación de salto si la tienes
        }

        // Flip (mirar en la dirección correcta)
        if (direccionMovimiento > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (direccionMovimiento < 0 && mirandoDerecha)
        {
            Girar();
        }
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
