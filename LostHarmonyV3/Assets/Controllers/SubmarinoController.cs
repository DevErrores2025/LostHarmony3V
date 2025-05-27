using System.Collections;
using UnityEngine;


public class SubmarinoController : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    public float velocidadHorizontal = 5f;
    public float velocidadVertical = 3f;

    [Header("Límites de Movimiento")]
    public float limiteArriba = -0.1f;
    public float limiteAbajo = -4.5f;

    [Header("Configuración Red")]
    public GameObject prefabRedLanzable;
    public float distanciaLanzamiento = 1f;
    public float velocidadRed = 15f;
    public float rangoDeteccion = 8f;
    // CAMBIO: Usar -1 (Everything) en lugar de LayerMask específico
    // public LayerMask capaEsbirros;
    public float cooldownRed = 0.8f;

    [Header("Configuración Vida")]
    public int vidaInicial = 3;
    public float tiempoInvulnerabilidad = 1f;
    public Color colorDaño = Color.red;
    public string tagEsbirro = "Esbirro";

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;
    private SpriteRenderer spriteRenderer;
    private int vidaActual;
    private bool esInvulnerable = false;
    private Color colorOriginal;
    private bool puedeLanzar = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }

        vidaActual = vidaInicial;
        Debug.Log("Submarino inicializado con " + vidaActual + " puntos de vida");
    }

    void Update()
    {
        // Movimiento
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        movimiento = new Vector2(
            movimientoHorizontal * velocidadHorizontal,
            movimientoVertical * velocidadVertical
        );

        if (animator != null)
        {
            animator.SetFloat("Velocidad", movimiento.magnitude);
        }

        if (movimientoHorizontal != 0)
        {
            float escalaX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(movimientoHorizontal);
            transform.localScale = new Vector3(escalaX, transform.localScale.y, transform.localScale.z);
        }

        // Control de la red
        if (Input.GetKeyDown(KeyCode.Space) && puedeLanzar)
        {
            LanzarRed();
            puedeLanzar = false;
            StartCoroutine(ResetearCooldown());
        }
    }

    IEnumerator ResetearCooldown()
    {
        yield return new WaitForSeconds(cooldownRed);
        puedeLanzar = true;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = movimiento;
        }
        AplicarLimites();
    }

    void LanzarRed()
    {
        if (prefabRedLanzable == null)
        {
            Debug.LogError("Prefab de red no asignado en el Inspector");
            return;
        }

        // CAMBIO: Buscar esbirros por tag en lugar de layer
        GameObject[] esbirrosGameObjects = GameObject.FindGameObjectsWithTag("Esbirro");
        Transform objetivo = null;
        float distanciaMinima = Mathf.Infinity;

        Debug.Log($"Esbirros encontrados: {esbirrosGameObjects.Length}");

        foreach (GameObject esbirroGO in esbirrosGameObjects)
        {
            // Verificar que el esbirro esté dentro del rango
            float distancia = Vector2.Distance(transform.position, esbirroGO.transform.position);
            if (distancia <= rangoDeteccion && distancia < distanciaMinima)
            {
                // Verificar que no haya sido atrapado ya
                CatchableObject catchable = esbirroGO.GetComponent<CatchableObject>();
                if (catchable == null || !catchable.FueAtrapado)
                {
                    distanciaMinima = distancia;
                    objetivo = esbirroGO.transform;
                }
            }
        }

        if (objetivo == null)
        {
            Debug.Log("No se encontraron esbirros válidos en el rango");
            return;
        }

        Debug.Log($"Lanzando red hacia esbirro a distancia: {distanciaMinima}");

        // Calcular posición de lanzamiento (bajo el submarino)
        Vector3 posicionLanzamiento = transform.position + new Vector3(0, -distanciaLanzamiento, 0);

        // Instanciar y configurar la red
        GameObject red = Instantiate(prefabRedLanzable, posicionLanzamiento, Quaternion.identity);
        RedLanzable redLanzable = red.GetComponent<RedLanzable>();

        if (redLanzable == null)
        {
            redLanzable = red.AddComponent<RedLanzable>();
        }

        redLanzable.Lanzar(objetivo.position, velocidadRed);

        // Trigger de animación
        if (animator != null)
        {
            animator.SetTrigger("LanzarRed");
        }
    }

    void AplicarLimites()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, limiteAbajo, limiteArriba);
        transform.position = pos;

        if (rb != null)
        {
            Vector2 vel = rb.linearVelocity;
            if (transform.position.y >= limiteArriba && vel.y > 0) vel.y = 0;
            else if (transform.position.y <= limiteAbajo && vel.y < 0) vel.y = 0;
            rb.linearVelocity = vel;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarColision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        ProcesarColision(collision.gameObject);
    }

    void ProcesarColision(GameObject objetoColisionado)
    {
        if ((objetoColisionado.CompareTag(tagEsbirro) && !esInvulnerable))
        {
            Debug.Log("Colisión con esbirro detectada");
            RecibirDaño(1);
        }
    }

    public void RecibirDaño(int cantidad)
    {
        if (esInvulnerable)
        {
            Debug.Log("Submarino es invulnerable, no recibe daño");
            return;
        }

        vidaActual -= cantidad;
        Debug.Log("Daño recibido. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Debug.Log("¡Submarino destruido!");
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(PeriodoInvulnerabilidad());
    }

    IEnumerator PeriodoInvulnerabilidad()
    {
        Debug.Log("Iniciando periodo de invulnerabilidad");
        esInvulnerable = true;

        if (spriteRenderer != null)
        {
            float tiempoPasado = 0;
            while (tiempoPasado < tiempoInvulnerabilidad)
            {
                spriteRenderer.color = (spriteRenderer.color == colorOriginal) ? colorDaño : colorOriginal;
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

    public int ObtenerVidaActual()
    {
        return vidaActual;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}