using UnityEngine;

public class RedLanzable : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeVida = 1.5f; // Tiempo máximo de vida si no impacta
    public float velocidadRotacion = 180f; // Rotación durante el vuelo

    private Vector3 direccion;
    private float velocidad;
    private bool haImpactado = false;
    private Rigidbody2D rb;
    private Transform objetivoActual;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Mejor detección
        }

        // Destrucción automática por tiempo
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        if (!haImpactado && objetivoActual != null)
        {
            // Rotar la red durante el vuelo
            transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);

            // Seguimiento suavizado al objetivo
            if (Vector3.Distance(transform.position, objetivoActual.position) > 0.5f)
            {
                direccion = (objetivoActual.position - transform.position).normalized;
                rb.linearVelocity = direccion * velocidad;
            }
        }
    }

    public void Lanzar(Vector3 objetivo, float velocidadRed)
    {
        // Buscar el transform del objetivo para seguimiento continuo
        objetivoActual = BuscarObjetivoMasCercano(objetivo);

        if (objetivoActual == null)
        {
            Destroy(gameObject);
            return;
        }

        direccion = (objetivoActual.position - transform.position).normalized;
        velocidad = velocidadRed;
        rb.linearVelocity = direccion * velocidad;
    }

    Transform BuscarObjetivoMasCercano(Vector3 posicionObjetivo)
    {
        // Buscar el esbirro más cercano al punto objetivo original
        Collider2D[] esbirros = Physics2D.OverlapCircleAll(posicionObjetivo, 5f, LayerMask.GetMask("Esbirros"));

        Transform objetivoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D esbirro in esbirros)
        {
            float distancia = Vector3.Distance(transform.position, esbirro.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetivoMasCercano = esbirro.transform;
            }
        }

        return objetivoMasCercano;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (haImpactado) return;

        if (other.CompareTag("Esbirro"))
        {
            haImpactado = true;

            CatchableObject esbirro = other.GetComponent<CatchableObject>();
            if (esbirro != null && !esbirro.FueAtrapado)
            {
                esbirro.SerAtrapado();

                // Efecto visual al atrapar
                if (TryGetComponent(out SpriteRenderer renderer))
                {
                    renderer.color = new Color(1, 1, 1, 0.7f); // Hacer semi-transparente
                }

                // Destruir después de breve tiempo para ver efecto
                Destroy(gameObject, 0.2f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("Red"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Opcional: Efecto de partículas al destruirse
        // Instantiate(efectoDestruccion, transform.position, Quaternion.identity);
    }
}