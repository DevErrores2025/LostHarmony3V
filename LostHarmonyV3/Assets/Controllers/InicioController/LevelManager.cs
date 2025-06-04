using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public int numeroDeNivel = 1; // Asignar en el Inspector según el nivel
    
    [Header("Escenas")]
    public string escenaMenuNiveles = "NivelesLostH"; // Cambia por el nombre de tu escena de selección de niveles
    public string escenaMenuPrincipal = "InicioDP";
    
    [Header("UI de Victoria (Opcional)")]
    public GameObject panelVictoria;
    public GameObject panelGameOver;

    // Mapeo de niveles a escenas
    private string[] escenasNiveles = {
        "", // Índice 0 vacío
        "Escena DP", // Nivel 1
        "MainCombat",      // Nivel 2
        "Alcantarillas"    // Nivel 3
    };

    void Start()
    {
        // Pausar el juego si hay panel de victoria activo
        if (panelVictoria != null && panelVictoria.activeInHierarchy)
        {
            Time.timeScale = 0f;
        }
    }

    // Llamar este método cuando el jugador complete el nivel
    public void NivelCompletado()
    {
        Debug.Log($"¡Nivel {numeroDeNivel} completado!");
        
        // Desbloquear el siguiente nivel
        LevelControllerLH.DesbloquearSiguienteNivel(numeroDeNivel);
        
        // Mostrar panel de victoria si existe
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Time.timeScale = 0f; // Pausar el juego
        }
        else
        {
            // Si no hay panel, ir directamente al siguiente nivel o menú
            SiguienteNivel();
        }
    }
    
    // Llamar cuando el jugador pierda
    public void GameOver()
    {
        Debug.Log("Game Over");
        
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    // Métodos para los botones de UI
    public void SiguienteNivel()
    {
        Time.timeScale = 1f; // Reanudar el tiempo
        
        int siguienteNivel = numeroDeNivel + 1;
        
        // Verificar si existe el siguiente nivel
        if (siguienteNivel < escenasNiveles.Length && !string.IsNullOrEmpty(escenasNiveles[siguienteNivel]))
        {
            SceneManager.LoadScene(escenasNiveles[siguienteNivel]);
        }
        else
        {
            // Si era el último nivel, volver al menú principal
            VolverAMenuPrincipal();
        }
    }
    
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void VolverAMenuNiveles()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuNiveles);
    }
    
    public void VolverAMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenuPrincipal);
    }
    
    // Método para pausar/despausar el juego
    public void PausarJuego()
    {
        Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
    }

    // Método para ir al siguiente nivel automáticamente (útil para transiciones fluidas)
    public void IrSiguienteNivelAutomatico()
    {
        Invoke("SiguienteNivel", 2f); // Esperar 2 segundos antes de cambiar
    }
}