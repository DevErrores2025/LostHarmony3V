using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre de la escena a la que quieres cambiar")]
    public string nombreEscena = "NombreDeTuEscena";
    
    [Header("Configuración de Interacción")]
    [Tooltip("Tag del objeto que puede activar el cambio (ej: Player)")]
    public string tagJugador = "Player";
    
    [Header("Efectos Opcionales")]
    [Tooltip("Sonido a reproducir antes del cambio (opcional)")]
    public AudioClip sonidoActivacion;
    
    private AudioSource audioSource;
    private bool puedeActivar = true;
    
    void Start()
    {
        // Obtener componente AudioSource si existe
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto que toca tiene el tag correcto
        if (other.CompareTag(tagJugador) && puedeActivar)
        {
            CambiarAEscena();
        }
    }
    
    // También puedes usar OnCollisionEnter2D si prefieres colisión en lugar de trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagJugador) && puedeActivar)
        {
            CambiarAEscena();
        }
    }
    
    void CambiarAEscena()
    {
        // Evitar múltiples activaciones
        puedeActivar = false;
        
        // Reproducir sonido si está configurado
        if (sonidoActivacion != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoActivacion);
            // Esperar un poco para que se reproduzca el sonido
            Invoke("CargarEscena", sonidoActivacion.length);
        }
        else
        {
            CargarEscena();
        }
    }
    
    void CargarEscena()
    {
        // Verificar que la escena existe en Build Settings
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError("La escena '" + nombreEscena + "' no está en Build Settings o no existe.");
        }
    }
    
    // Método alternativo para cambiar por índice de escena
    public void CambiarPorIndice(int indiceEscena)
    {
        if (indiceEscena >= 0 && indiceEscena < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(indiceEscena);
        }
        else
        {
            Debug.LogError("Índice de escena inválido: " + indiceEscena);
        }
    }
}