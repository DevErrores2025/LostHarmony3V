using UnityEngine;

public class CatchableObject : MonoBehaviour
{
    public bool FueAtrapado { get; private set; } = false;
    private EsbirroMovement _movement;

    void Start()
    {
        gameObject.tag = "Esbirro";
        _movement = GetComponent<EsbirroMovement>();
        ConfigurarColliders();
    }

    void ConfigurarColliders()
    {
        var collider2D = GetComponent<Collider2D>();
        if (collider2D != null) collider2D.isTrigger = true;
    }
    void OnDestroy()
    {
        // Si no fue atrapado y aún así se destruyó, asumimos que fue esquivado
        if (!FueAtrapado)
        {
            GameManagerDP.Instance?.EsbirroEsquivado();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (FueAtrapado) return;

        if (other.CompareTag("Net") || other.CompareTag("Red"))
        {
            SerAtrapado();
        }
        else if (other.CompareTag("Player"))
        {
            // Lógica de daño al jugador
        }
    }
    
    public void SerAtrapado()
    {
        FueAtrapado = true;
        _movement?.StopMovement();

        // Añadir puntuación directamente al GameManager
        GameManagerDP.Instance?.AddScore(10);

        // Efecto visual
        if (TryGetComponent(out SpriteRenderer renderer))
        {
            renderer.color = new Color(1, 1, 1, 0.5f);
        }

        Destroy(gameObject, 0.2f);
    }
}