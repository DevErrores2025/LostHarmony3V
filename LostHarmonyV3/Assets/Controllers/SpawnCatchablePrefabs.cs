using UnityEngine;

public class SpawnCatchablePrefabs : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab;
    public GameObject prefab1; // Asignar el prefab "ESBIRRO FELIZ (1)" en el Inspector

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public float horizontalSpawnOffset = 7f; // Distancia horizontal desde el jugador
    public float verticalSpawnRange = 2f; // Rango vertical aleatorio
    public bool relativeToPlayer = true;

    [Header("Prefab Selection")]
    [Range(0f, 1f)]
    public float prefab1Probability = 0.3f; // 30% de probabilidad para prefab1
    public bool alternateSpawn = false; // Si es true, alterna entre prefabs
    public bool randomSpawn = true; // Si es true, selecciona aleatoriamente basado en probabilidad

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Vector2 movementDirection = new Vector2(-1f, -0.5f).normalized;

    private float nextSpawnTime;
    private Transform playerTransform;
    private Vector3 playerInitialPosition;
    private bool useNextPrefab = false; // Para modo alternado

    void Start()
    {
        // Validar que al menos un prefab esté asignado
        if (prefab == null && prefab1 == null)
        {
            Debug.LogError("Ningún prefab asignado en el Inspector", this);
            enabled = false;
            return;
        }

        // Si solo hay un prefab, ajustar configuración
        if (prefab == null || prefab1 == null)
        {
            randomSpawn = false;
            alternateSpawn = false;
            Debug.LogWarning("Solo un prefab asignado. Modo aleatorio/alternado desactivado.", this);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerInitialPosition = playerTransform.position;
            Debug.Log($"Posición inicial del jugador: {playerInitialPosition}");
        }
        else
        {
            Debug.LogError("Jugador no encontrado", this);
            enabled = false;
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && playerTransform != null)
        {
            GameObject prefabToSpawn = SeleccionarPrefab();
            if (prefabToSpawn != null)
            {
                SpawnObject(prefabToSpawn);
            }
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    GameObject SeleccionarPrefab()
    {
        // Si solo hay un prefab disponible
        if (prefab == null) return prefab1;
        if (prefab1 == null) return prefab;

        // Modo alternado
        if (alternateSpawn && !randomSpawn)
        {
            useNextPrefab = !useNextPrefab;
            return useNextPrefab ? prefab1 : prefab;
        }

        // Modo aleatorio basado en probabilidad
        if (randomSpawn)
        {
            float randomValue = Random.Range(0f, 1f);
            return randomValue <= prefab1Probability ? prefab1 : prefab;
        }

        // Por defecto usar prefab
        return prefab;
    }

    void SpawnObject(GameObject prefabToSpawn)
    {
        Vector3 spawnPos = CalculateSpawnPosition();
        Debug.Log($"Generando esbirro en: {spawnPos} usando prefab: {prefabToSpawn.name}");

        GameObject newObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        if (newObj == null) return;

        ConfigureEsbirro(newObj, prefabToSpawn);
    }

    Vector3 CalculateSpawnPosition()
    {
        Vector3 referencePosition = relativeToPlayer ? playerTransform.position : playerInitialPosition;

        // Calcular posición relativa al jugador
        float spawnX = referencePosition.x + horizontalSpawnOffset;
        float spawnY = referencePosition.y + Random.Range(-verticalSpawnRange, verticalSpawnRange);

        return new Vector3(spawnX, spawnY, -1f); // Z=-1 para objetos 2D
    }

    void ConfigureEsbirro(GameObject esbirro, GameObject prefabOriginal)
    {
        // Configurar movimiento
        var movement = esbirro.AddComponent<EsbirroMovement>();
        movement.Initialize(movementDirection, moveSpeed, playerTransform);

        // Configurar velocidades diferentes según el prefab si es necesario
        if (prefabOriginal == prefab1)
        {
            // Hacer que prefab1 sea ligeramente más rápido o tenga comportamiento diferente
            movement.ConfigurarVelocidad(moveSpeed * 1.1f);
            Debug.Log($"Configurando {prefabOriginal.name} con velocidad aumentada");
        }

        // Configurar física
        var rb2d = esbirro.GetComponent<Rigidbody2D>() ?? esbirro.AddComponent<Rigidbody2D>();
        rb2d.gravityScale = 0f;
        rb2d.isKinematic = true;

        // Configurar collider
        var collider = esbirro.GetComponent<Collider2D>() ?? esbirro.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // Configurar tags y componentes
        esbirro.tag = "Esbirro";

        // Verificar si ya tiene CatchableObject antes de añadirlo
        if (esbirro.GetComponent<CatchableObject>() == null)
        {
            esbirro.AddComponent<CatchableObject>();
        }
    }

    // Métodos públicos para control externo
    public void CambiarProbabilidadPrefab1(float nuevaProbabilidad)
    {
        prefab1Probability = Mathf.Clamp01(nuevaProbabilidad);
    }

    public void ActivarModoAleatorio(bool activar)
    {
        randomSpawn = activar;
        if (activar) alternateSpawn = false; // Desactivar alternado si se activa aleatorio
    }

    public void ActivarModoAlternado(bool activar)
    {
        alternateSpawn = activar;
        if (activar) randomSpawn = false; // Desactivar aleatorio si se activa alternado
    }

    public void CambiarVelocidadSpawn(float nuevaVelocidad)
    {
        spawnInterval = Mathf.Max(0.1f, nuevaVelocidad);
    }

    // Información de debug
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Vector3 referencePos = relativeToPlayer ? playerTransform.position : playerInitialPosition;

            // Dibujar área de spawn
            Gizmos.color = Color.yellow;
            Vector3 spawnCenter = new Vector3(referencePos.x + horizontalSpawnOffset, referencePos.y, referencePos.z);
            Gizmos.DrawWireCube(spawnCenter, new Vector3(0.5f, verticalSpawnRange * 2f, 0.1f));

            // Dibujar línea desde jugador al área de spawn
            Gizmos.color = Color.green;
            Gizmos.DrawLine(referencePos, spawnCenter);
        }
    }
}