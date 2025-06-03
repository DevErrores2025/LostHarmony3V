using UnityEngine;

public class Controller2d : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce = 15;

    private Rigidbody2D rb;
    private Transform groundCheck;
    public Animator animator;
    private float _radius = 0.2f;
    private bool hit;
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 12f;
    public Transform verificadorSuelo;
    public LayerMask capaSuelo;

    private bool enSuelo = false;
    private float direccionMovimiento;
    private bool mirandoDerecha = true;



    private float defaultGravityScale;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = GameObject.Find("GroundCheck").transform;

        defaultGravityScale = rb.gravityScale;  // Guardamos la gravedad original
    }

    void FixedUpdate()
    {
        direccionMovimiento = getHorizontal();
        hit = Physics2D.OverlapCircle(groundCheck.position, _radius, layerMask);

        Vector2 velocity = new Vector2(direccionMovimiento * moveSpeed, rb.linearVelocity.y);

        if (IsJump() && hit)
        {
            velocity.y = jumpForce;
        }

        rb.linearVelocity = velocity;

        // Animaciones
        if (enSuelo)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));
        }
        else
        {
            animator.SetFloat("Velocidad", Mathf.Abs(direccionMovimiento));
        }

        // Flip
        if (direccionMovimiento > 0 && mirandoDerecha)
        {
            Girar();
        }
        else if (direccionMovimiento < 0 && !mirandoDerecha)
        {
            Girar();
        }
    }

    public float getHorizontal()
    {
        bool touchLect = false;
        bool touchRight = false;

        foreach (Touch touch in Input.touches)
        {
            if (touch.position.x < Screen.width * 1 / 4 && touch.position.y < Screen.height * 1 / 2)
            {
                touchLect = true;
            }
            else if (touch.position.x > Screen.width * 1 / 4 && touch.position.x < Screen.width * 2 / 4 && touch.position.y < Screen.height * 1 / 2)
            {
                touchRight = true;
            }
        }
        return Input.GetAxis("Horizontal") + (touchLect ? -1f : 0f) + (touchRight ? 1f : 0f);
    }

    public bool IsJump()
    {
        return Input.GetKey(KeyCode.Space);
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

// Detectar cuando entra al �rea
private void OnTriggerEnter2D(Collider2D collision)
    {
        // Puedes filtrar con etiquetas o capas si quieres que solo ciertos triggers inviertan la gravedad
        if (collision.CompareTag("GravityArea"))
        {
            rb.gravityScale = -Mathf.Abs(defaultGravityScale);  // Gravedad negativa
        }
    }

    // Detectar cuando sale del �rea
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GravityArea"))
        {
            rb.gravityScale = defaultGravityScale;  // Volver a la gravedad normal
        }
    }
}

