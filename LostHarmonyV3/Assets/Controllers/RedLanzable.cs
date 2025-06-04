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
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Asegurar que tenga un collider trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = true;

        // Asignar tag si no lo tiene
        if (gameObject.tag == "Untagged")
        {
            gameObject.tag = "Red";
        }

        // Destrucción automática por tiempo
        Destroy(gameObject, tiempoDeVida);

        Debug.Log("Red creada y configurada");
    }

    void Update()
    {
        if (!haImpactado)
        {
            // Rotar la red durante el vuelo
            transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);

            // Si tenemos objetivo, seguirlo
            if (objetivoActual != null)
            {
                // Verificar si el objetivo sigue siendo válido
                CatchableObject catchable = objetivoActual.GetComponent<CatchableObject>();
                if (catchable != null && catchable.FueAtrapado)
                {
                    // El objetivo ya fue atrapado, buscar otro o destruirse
                    BuscarNuevoObjetivo();
                }
                else if (Vector3.Distance(transform.position, objetivoActual.position) > 0.5f)
                {
                    direccion = (objetivoActual.position - transform.position).normalized;
                    rb.linearVelocity = direccion * velocidad;
                }
            }
        }
    }

    void BuscarNuevoObjetivo()
    {
        GameObject[] esbirros = GameObject.FindGameObjectsWithTag("Esbirro");
        Transform nuevoObjetivo = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (GameObject esbirro in esbirros)
        {
            CatchableObject catchable = esbirro.GetComponent<CatchableObject>();
            if (catchable != null && !catchable.FueAtrapado)
            {
                float distancia = Vector3.Distance(transform.position, esbirro.transform.position);
                if (distancia < distanciaMinima && distancia <= 5f) // Rango máximo para cambio de objetivo
                {
                    distanciaMinima = distancia;
                    nuevoObjetivo = esbirro.transform;
                }
            }
        }

        objetivoActual = nuevoObjetivo;
        if (objetivoActual == null)
        {
            Debug.Log("No hay más objetivos válidos, destruyendo red");
            Destroy(gameObject, 0.5f);
        }
    }

    public void Lanzar(Vector3 objetivo, float velocidadRed)
    {
        Debug.Log($"Lanzando red hacia: {objetivo} con velocidad: {velocidadRed}");

        // Buscar el transform del objetivo para seguimiento continuo
        objetivoActual = BuscarObjetivoMasCercano(objetivo);

        if (objetivoActual == null)
        {
            Debug.Log("No se encontró objetivo válido para la red");
            Destroy(gameObject);
            return;
        }

        direccion = (objetivoActual.position - transform.position).normalized;
        velocidad = velocidadRed;
        rb.linearVelocity = direccion * velocidad;

        Debug.Log($"Red lanzada hacia: {objetivoActual.name}");
    }

    Transform BuscarObjetivoMasCercano(Vector3 posicionObjetivo)
    {
        GameObject[] esbirros = GameObject.FindGameObjectsWithTag("Esbirro");
        Transform objetivoMasCercano = null;
        float distanciaMinima = Mathf.Infinity;

        Debug.Log($"Buscando entre {esbirros.Length} esbirros");

        foreach (GameObject esbirro in esbirros)
        {
            // Verificar que no haya sido atrapado
            CatchableObject catchable = esbirro.GetComponent<CatchableObject>();
            if (catchable != null && catchable.FueAtrapado)
                continue;

            float distancia = Vector3.Distance(transform.position, esbirro.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetivoMasCercano = esbirro.transform;
            }
        }

        if (objetivoMasCercano != null)
        {
            Debug.Log($"Objetivo encontrado: {objetivoMasCercano.name} a distancia: {distanciaMinima}");
        }

        return objetivoMasCercano;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (haImpactado) return;

        Debug.Log($"Red colisionó con: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Esbirro"))
        {
            haImpactado = true;

            CatchableObject esbirro = other.GetComponent<CatchableObject>();
            if (esbirro != null && !esbirro.FueAtrapado)
            {
                Debug.Log("¡Esbirro atrapado!");
                esbirro.SerAtrapado();

                // Efecto visual al atrapar
                if (TryGetComponent(out SpriteRenderer renderer))
                {
                    renderer.color = new Color(1, 1, 1, 0.7f);
                }

                // Destruir después de breve tiempo para ver efecto
                Destroy(gameObject, 0.2f);
            }
            else
            {
                Debug.Log("Esbirro ya fue atrapado o no tiene CatchableObject");
                Destroy(gameObject);
            }
        }
        else if (!other.CompareTag("Player") && !other.CompareTag("Red"))
        {
            Debug.Log("Red colisionó con objeto no válido, destruyendo");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log("Red destruida");
    }
}