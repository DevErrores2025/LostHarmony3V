using UnityEngine;

public class PlayerMovementEFB : MonoBehaviour
{
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 12f;
    public Transform verificadorSuelo;
    public LayerMask capaSuelo;
    public Animator animator;
    private Rigidbody2D rb;

    private bool enSuelo = false;
    private float direccionMovimiento;
    private bool mirandoDerecha = false;

    private JugadorVida jugadorVida;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jugadorVida = GetComponent<JugadorVida>();
    }

    void Update()
    {
        // ✅ Bloquear movimiento si está muerto
        if (jugadorVida != null && jugadorVida.estaMuerto)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

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
        animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));

        // Girar personaje
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
