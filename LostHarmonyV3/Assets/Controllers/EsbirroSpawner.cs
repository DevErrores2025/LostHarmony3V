using UnityEngine;
using System.Collections;

public class EsbirroSpawner : MonoBehaviour
{
    [Header("Configuración Spawn")]
    public GameObject prefabEsbirro;
    public Transform[] puntosSpawn; // Diferentes puntos donde pueden aparecer los esbirros
    public float intervaloSpawnFase1 = 2f;
    public float intervaloSpawnFase2 = 1.5f; // Más rápido en fase 2

    [Header("Límites de Spawn")]
    public int maxEsbirrosSimultaneos = 3;
    public float limiteInferiorY = -5f; // Los esbirros se destruyen si pasan este límite

    private bool spawneando = false;
    private bool esFase2 = false;
    private int esbirrosActivos = 0;

    void Start()
    {
        // Si no se asignaron puntos de spawn, crear algunos por defecto
        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            CrearPuntosSpawnPorDefecto();
        }

        IniciarSpawn();
    }

    void CrearPuntosSpawnPorDefecto()
    {
        // Crear puntos de spawn en la parte superior de la pantalla
        GameObject contenedorPuntos = new GameObject("PuntosSpawn");
        contenedorPuntos.transform.SetParent(transform);

        puntosSpawn = new Transform[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject punto = new GameObject($"PuntoSpawn_{i}");
            punto.transform.SetParent(contenedorPuntos.transform);
            punto.transform.position = new Vector3(-6f + (i * 6f), 5f, 0f);
            puntosSpawn[i] = punto.transform;
        }
    }

    public void IniciarSpawn()
    {
        if (spawneando) return;

        spawneando = true;
        StartCoroutine(SpawnearEsbirros());
    }

    public void DetenerSpawn()
    {
        spawneando = false;
        Debug.Log("Spawn de esbirros detenido");
    }

    public void CambiarAFase2()
    {
        esFase2 = true;
        Debug.Log("Spawner cambiado a Fase 2 - Intervalo más rápido");
    }

    IEnumerator SpawnearEsbirros()
    {
        while (spawneando)
        {
            // Solo spawnear si no hemos alcanzado el máximo de esbirros simultáneos
            if (esbirrosActivos < maxEsbirrosSimultaneos && prefabEsbirro != null)
            {
                SpawnearEsbirro();
            }

            // Usar intervalo apropiado según la fase
            float intervalo = esFase2 ? intervaloSpawnFase2 : intervaloSpawnFase1;
            yield return new WaitForSeconds(intervalo);
        }
    }

    void SpawnearEsbirro()
    {
        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn configurados");
            return;
        }

        // Seleccionar punto de spawn aleatorio
        Transform puntoSpawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

        // Instanciar esbirro
        GameObject nuevoEsbirro = Instantiate(prefabEsbirro, puntoSpawn.position, Quaternion.identity);

        // Configurar el esbirro
        EsbirroMovement movimiento = nuevoEsbirro.GetComponent<EsbirroMovement>();
        if (movimiento == null)
        {
            movimiento = nuevoEsbirro.AddComponent<EsbirroMovement>();
        }

        // Configurar velocidad según la fase
        float velocidad = esFase2 ? 4f : 3f;
        movimiento.ConfigurarVelocidad(velocidad);
        movimiento.ConfigurarOrigenAbanico(false); // No viene del abanico

        // Asegurar que tenga CatchableObject
        if (nuevoEsbirro.GetComponent<CatchableObject>() == null)
        {
            nuevoEsbirro.AddComponent<CatchableObject>();
        }

        // Configurar destrucción automática cuando pase el límite inferior
        EsbirroAutoDestroy autoDestroy = nuevoEsbirro.GetComponent<EsbirroAutoDestroy>();
        if (autoDestroy == null)
        {
            autoDestroy = nuevoEsbirro.AddComponent<EsbirroAutoDestroy>();
        }
        autoDestroy.ConfigurarLimite(limiteInferiorY, this);

        esbirrosActivos++;
        Debug.Log($"Esbirro spawneado. Activos: {esbirrosActivos}");
    }

    public void EsbirroDestruido()
    {
        esbirrosActivos--;
        if (esbirrosActivos < 0) esbirrosActivos = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (puntosSpawn != null)
        {
            Gizmos.color = Color.blue;
            foreach (Transform punto in puntosSpawn)
            {
                if (punto != null)
                {
                    Gizmos.DrawWireSphere(punto.position, 0.5f);
                }
            }
        }

        // Dibujar línea del límite inferior
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10f, limiteInferiorY, 0f), new Vector3(10f, limiteInferiorY, 0f));
    }
}

// Componente auxiliar para manejar la destrucción automática de esbirros
public class EsbirroAutoDestroy : MonoBehaviour
{
    private float limiteY;
    private EsbirroSpawner spawner;

    public void ConfigurarLimite(float limite, EsbirroSpawner spawnerRef)
    {
        limiteY = limite;
        spawner = spawnerRef;
    }

    void Update()
    {
        if (transform.position.y < limiteY)
        {
            if (spawner != null)
            {
                spawner.EsbirroDestruido();
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EsbirroDestruido();
        }
    }
}