using UnityEngine;
using System.Collections;

public class SubmarinoController: MonoBehaviour
{
    [Header("Configuraci�n Movimiento")]
    public float velocidadHorizontal = 5f;
    public float velocidadVertical = 3f;

    [Header("Configuraci�n Red")]
    public Transform red;
    public float distanciaRed = 1f;

    [Header("Configuraci�n Vida")]
    public int vidaInicial = 3;
    public float tiempoInvulnerabilidad = 1f;
    public Color colorDa�o = Color.red;
    public string tagEsbirro = "Esbirro"; // Define el tag que usar�n los esbirros

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;
    private SpriteRenderer spriteRenderer;
    private int vidaActual;
    private bool esInvulnerable = false;
    private Color colorOriginal;

    void Awake()
    {
        // Obtener componentes una sola vez al inicio
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Configurar Rigidbody2D si existe
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Guardar color original
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }

        // Inicializar vida
        vidaActual = vidaInicial;

        Debug.Log("Submarino inicializado con " + vidaActual + " puntos de vida");
    }

    void Update()
    {
        // Calcular movimiento en Update (mejor respuesta a input)
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        // Guardar el vector de movimiento para usar en FixedUpdate
        movimiento = new Vector2(
            movimientoHorizontal * velocidadHorizontal,
            movimientoVertical * velocidadVertical
        );

        // Actualizar animaci�n
        if (animator != null)
        {
            animator.SetFloat("Velocidad", movimiento.magnitude);
        }

        // Orientar el sprite SIN cambiar su escala (solo invertir)
        if (movimientoHorizontal != 0)
        {
            // Solo cambiamos el signo de la escala X, manteniendo su valor absoluto
            float escalaX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(movimientoHorizontal);
            transform.localScale = new Vector3(escalaX, transform.localScale.y, transform.localScale.z);
        }

        // Debug - Mostrar vida actual cada 5 segundos
        if (Time.frameCount % 300 == 0)
        {
            Debug.Log("Estado del Submarino - Vida actual: " + vidaActual + ", Invulnerable: " + esInvulnerable);
        }
    }

    void FixedUpdate()
    {
        // Aplicar movimiento en FixedUpdate (mejor para f�sicas)
        if (rb != null)
        {
            rb.linearVelocity = movimiento;
        }

        // Actualizar posici�n de la red
        ActualizarPosicionRed();
    }

    void ActualizarPosicionRed()
    {
        if (red != null)
        {
            // Actualizar posici�n de la red
            red.position = new Vector3(
                transform.position.x,
                transform.position.y - distanciaRed,
                red.position.z
            );
        }
    }

    // Detectar colisiones con m�todos diferentes para asegurar que funcione

    // 1. Para colisiones normales (Collider y Rigidbody)
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLISI�N 2D detectada con: " + collision.gameObject.name + ", Tag: " + collision.gameObject.tag);

        // Manejar colisi�n con paredes
        if (collision.contactCount > 0 && rb != null)
        {
            // C�digo para manejar colisiones con paredes
            Vector2 normal = collision.contacts[0].normal;
            Vector2 velocidadActual = rb.linearVelocity;

            if (Mathf.Abs(normal.x) > 0.5f)
            {
                velocidadActual.x = 0;
            }
            if (Mathf.Abs(normal.y) > 0.5f)
            {
                velocidadActual.y = 0;
            }

            rb.linearVelocity = velocidadActual;
        }

        // Verificar colisi�n con esbirro por nombre o tag
        if ((collision.gameObject.CompareTag(tagEsbirro) ||
             collision.gameObject.name.Contains("ESBIRRO")) &&
            !esInvulnerable)
        {
            Debug.Log("�COLISI�N V�LIDA! Detectado esbirro por colisi�n normal");
            RecibirDa�o(1);
        }
    }

    // Tambi�n probar con OnCollisionEnter (3D) por si acaso
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLISI�N 3D detectada con: " + collision.gameObject.name + ", Tag: " + collision.gameObject.tag);

        // Verificar colisi�n con esbirro por nombre o tag
        if ((collision.gameObject.CompareTag(tagEsbirro) ||
             collision.gameObject.name.Contains("ESBIRRO")) &&
            !esInvulnerable)
        {
            Debug.Log("�COLISI�N V�LIDA! Detectado esbirro por colisi�n 3D");
            RecibirDa�o(1);
        }
    }

    // 2. Para triggers (Is Trigger = true)
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER 2D detectado con: " + collision.name + ", Tag: " + collision.tag);

        // Verificar colisi�n con esbirro por nombre o tag
        if ((collision.CompareTag(tagEsbirro) ||
             collision.name.Contains("ESBIRRO")) &&
            !esInvulnerable)
        {
            Debug.Log("�TRIGGER V�LIDO! Detectado esbirro por trigger 2D");
            RecibirDa�o(1);
        }
    }

    // Tambi�n probar con OnTriggerEnter (3D) por si acaso
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("TRIGGER 3D detectado con: " + collision.name + ", Tag: " + collision.tag);

        // Verificar colisi�n con esbirro por nombre o tag
        if ((collision.CompareTag(tagEsbirro) ||
             collision.name.Contains("ESBIRRO")) &&
            !esInvulnerable)
        {
            Debug.Log("�TRIGGER V�LIDO! Detectado esbirro por trigger 3D");
            RecibirDa�o(1);
        }
    }

    // M�todo para recibir da�o
    public void RecibirDa�o(int cantidad)
    {
        if (esInvulnerable)
        {
            Debug.Log("Submarino es invulnerable, no recibe da�o");
            return;
        }

        vidaActual -= cantidad;
        Debug.Log("�DA�O RECIBIDO! Vida actual: " + vidaActual + "/" + vidaInicial);

        // Comprueba si el jugador ha muerto
        if (vidaActual <= 0)
        {
            Debug.Log("�El submarino ha sido destruido!");
            // Opcional: desactivar el objeto
            gameObject.SetActive(false);
            return;
        }

        // Activar invulnerabilidad temporal
        StartCoroutine(PeriodoInvulnerabilidad());
    }

    // Coroutine para invulnerabilidad
    IEnumerator PeriodoInvulnerabilidad()
    {
        Debug.Log("Iniciando periodo de invulnerabilidad (" + tiempoInvulnerabilidad + " segundos)");
        esInvulnerable = true;

        // Efecto visual de parpadeo
        if (spriteRenderer != null)
        {
            float tiempoPasado = 0;
            while (tiempoPasado < tiempoInvulnerabilidad)
            {
                spriteRenderer.color = (spriteRenderer.color == colorOriginal) ? colorDa�o : colorOriginal;
                yield return new WaitForSeconds(0.1f);
                tiempoPasado += 0.1f;
            }

            spriteRenderer.color = colorOriginal;
        }
        else
        {
            yield return new WaitForSeconds(tiempoInvulnerabilidad);
        }

        esInvulnerable = false;
        Debug.Log("Periodo de invulnerabilidad terminado");
    }

    // M�todo para obtener la vida actual
    public int ObtenerVidaActual()
    {
        return vidaActual;
    }
}