using UnityEngine;

public class SpawnCatchablePrefabs : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab; // Asignar el prefab "ESBIRRO FELIZ (1)" en el Inspector

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public float horizontalSpawnOffset = 7f; // Distancia horizontal desde el jugador
    public float verticalSpawnRange = 2f; // Rango vertical aleatorio
    public bool relativeToPlayer = true;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Vector2 movementDirection = new Vector2(-1f, -0.5f).normalized;

    private float nextSpawnTime;
    private Transform playerTransform;
    private Vector3 playerInitialPosition;

    void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab no asignado en el Inspector", this);
            enabled = false;
            return;
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
        if (Time.time >= nextSpawnTime && prefab != null && playerTransform != null)
        {
            SpawnObject();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnObject()
    {
        Vector3 spawnPos = CalculateSpawnPosition();
        Debug.Log($"Generando esbirro en: {spawnPos}");

        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        if (newObj == null) return;

        ConfigureEsbirro(newObj);
    }

    Vector3 CalculateSpawnPosition()
    {
        Vector3 referencePosition = relativeToPlayer ? playerTransform.position : playerInitialPosition;

        // Calcular posición relativa al jugador
        float spawnX = referencePosition.x + horizontalSpawnOffset;
        float spawnY = referencePosition.y + Random.Range(-verticalSpawnRange, verticalSpawnRange);

        return new Vector3(spawnX, spawnY, -1f); // Z=-1 para objetos 2D
    }

    void ConfigureEsbirro(GameObject esbirro)
    {
        // Configurar movimiento
        var movement = esbirro.AddComponent<EsbirroMovement>();
        movement.Initialize(movementDirection, moveSpeed, playerTransform);

        // Configurar física
        var rb2d = esbirro.GetComponent<Rigidbody2D>() ?? esbirro.AddComponent<Rigidbody2D>();
        rb2d.gravityScale = 0f;
        rb2d.isKinematic = true;

        // Configurar collider
        var collider = esbirro.GetComponent<Collider2D>() ?? esbirro.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // Configurar tags y componentes
        esbirro.tag = "Esbirro";
        esbirro.AddComponent<CatchableObject>();
    }
}