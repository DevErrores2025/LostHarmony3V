using UnityEngine;
using System.Collections;

public class DamaPlasticaManager : MonoBehaviour
{
    [Header("Configuración Aparición")]
    public float tiempoAparicion = 1.5f;
    public Vector3 posicionFinal = new Vector3(8f, 2f, 0f); // Posición donde se posiciona la dama
    public Vector3 posicionInicial = new Vector3(15f, 2f, 0f); // Fuera de pantalla

    [Header("Configuración Abanico")]
    public Transform puntoLanzamientoAbanico; // Punto desde donde salen los esbirros del abanico
    public GameObject prefabEsbirro;
    public float intervaloLanzamiento = 2f;
    public float velocidadEsbirroAbanico = 3f;
    public int cantidadEsbirrosAbanico = 5; // Cuántos esbirros lanza desde el abanico

    [Header("Configuración Animación")]
    public Animator animator;

    private bool estaActiva = false;
    private bool estaLanzando = false;
    private int esbirrosLanzados = 0;

    void Start()
    {
        // Inicializar en posición fuera de pantalla
        transform.position = posicionInicial;
        gameObject.SetActive(false);

        // Si no se asignó el punto de lanzamiento, usar la posición de la dama
        if (puntoLanzamientoAbanico == null)
        {
            // Crear un punto de lanzamiento ligeramente adelante de la dama
            GameObject puntoLanzamiento = new GameObject("PuntoLanzamientoAbanico");
            puntoLanzamiento.transform.SetParent(transform);
            puntoLanzamiento.transform.localPosition = new Vector3(-1f, 0f, 0f);
            puntoLanzamientoAbanico = puntoLanzamiento.transform;
        }
    }

    public void AparecerEnEscena()
    {
        if (estaActiva) return;

        Debug.Log("La Dama de Plástico aparece en escena!");
        gameObject.SetActive(true);
        estaActiva = true;

        StartCoroutine(AnimacionAparicion());
    }

    IEnumerator AnimacionAparicion()
    {
        // Mover desde posición inicial hasta posición final
        float tiempo = 0;
        Vector3 posInicial = transform.position;

        // Trigger de animación de aparición si existe
        if (animator != null)
        {
            animator.SetTrigger("Aparecer");
        }

        while (tiempo < tiempoAparicion)
        {
            transform.position = Vector3.Lerp(posInicial, posicionFinal, tiempo / tiempoAparicion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = posicionFinal;

        // Comenzar a lanzar esbirros después de aparecer
        yield return new WaitForSeconds(0.5f);
        IniciarLanzamientoEsbirros();
    }

    void IniciarLanzamientoEsbirros()
    {
        if (estaLanzando) return;

        Debug.Log("La Dama comienza a lanzar esbirros desde su abanico!");
        estaLanzando = true;
        esbirrosLanzados = 0;

        // Trigger de animación de lanzamiento continuo
        if (animator != null)
        {
            animator.SetBool("EstáLanzando", true);
        }

        StartCoroutine(LanzarEsbirrosDelAbanico());
    }

    IEnumerator LanzarEsbirrosDelAbanico()
    {
        while (estaLanzando && esbirrosLanzados < cantidadEsbirrosAbanico)
        {
            LanzarEsbirroDelAbanico();
            esbirrosLanzados++;

            // Trigger de animación para cada lanzamiento
            if (animator != null)
            {
                animator.SetTrigger("LanzarEsbirro");
            }

            yield return new WaitForSeconds(intervaloLanzamiento);
        }

        // Detener animación de lanzamiento
        if (animator != null)
        {
            animator.SetBool("EstáLanzando", false);
        }

        Debug.Log($"La Dama terminó de lanzar {esbirrosLanzados} esbirros del abanico");
    }

    void LanzarEsbirroDelAbanico()
    {
        if (prefabEsbirro == null || puntoLanzamientoAbanico == null)
        {
            Debug.LogError("Falta prefab de esbirro o punto de lanzamiento en DamaPlasticaManager");
            return;
        }

        // Instanciar esbirro en el punto de lanzamiento del abanico
        GameObject nuevoEsbirro = Instantiate(prefabEsbirro, puntoLanzamientoAbanico.position, Quaternion.identity);

        // Configurar el esbirro para que se mueva hacia el jugador
        EsbirroMovement movimiento = nuevoEsbirro.GetComponent<EsbirroMovement>();
        if (movimiento != null)
        {
            // Configurar velocidad específica para esbirros del abanico
            movimiento.ConfigurarVelocidad(velocidadEsbirroAbanico);
            movimiento.ConfigurarOrigenAbanico(true); // Marcar que viene del abanico
        }

        // Asegurar que tenga el componente CatchableObject
        if (nuevoEsbirro.GetComponent<CatchableObject>() == null)
        {
            nuevoEsbirro.AddComponent<CatchableObject>();
        }

        Debug.Log("Esbirro lanzado desde el abanico de la Dama");
    }

    public void Retirarse()
    {
        if (!estaActiva) return;

        Debug.Log("La Dama de Plástico se retira");

        // Detener lanzamiento si está activo
        estaLanzando = false;

        // Trigger de animación de retirada
        if (animator != null)
        {
            animator.SetTrigger("Retirarse");
            animator.SetBool("EstáLanzando", false);
        }

        StartCoroutine(AnimacionRetirada());
    }

    IEnumerator AnimacionRetirada()
    {
        // Mover desde posición actual hasta fuera de pantalla
        float tiempo = 0;
        Vector3 posInicial = transform.position;
        Vector3 posicionSalida = new Vector3(15f, posInicial.y, posInicial.z);

        while (tiempo < tiempoAparicion)
        {
            transform.position = Vector3.Lerp(posInicial, posicionSalida, tiempo / tiempoAparicion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        // Desactivar la dama
        estaActiva = false;
        gameObject.SetActive(false);
        Debug.Log("La Dama se ha retirado completamente");
    }

    // Método para forzar detener el lanzamiento (útil si el jugador gana antes)
    public void DetenerLanzamiento()
    {
        estaLanzando = false;
        if (animator != null)
        {
            animator.SetBool("EstáLanzando", false);
        }
    }

    // Getters para información del estado
    public bool EstaActiva => estaActiva;
    public bool EstaLanzando => estaLanzando;
    public int EsbirrosLanzados => esbirrosLanzados;

    void OnDrawGizmosSelected()
    {
        // Dibujar posiciones de aparición y final
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(posicionFinal, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(posicionInicial, 0.5f);

        // Dibujar punto de lanzamiento del abanico
        if (puntoLanzamientoAbanico != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(puntoLanzamientoAbanico.position, 0.3f);
        }
    }
}