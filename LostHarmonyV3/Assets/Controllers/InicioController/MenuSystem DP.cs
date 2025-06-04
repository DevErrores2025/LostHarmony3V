using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [Header("Nombres de Escenas - VERIFICA ESTOS NOMBRES")]
    [Tooltip("Arrastra la escena desde el Project window o escribe el nombre exacto")]
    public string escenaSeleccionNiveles = "NivelesLostH"; 
    public string escenaPrimerNivel = "Escena DP"; 
    
    void Start()
    {
        // Debug para mostrar todas las escenas disponibles
        Debug.Log("=== ESCENAS DISPONIBLES EN BUILD SETTINGS ===");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"Índice {i}: {sceneName} (Path: {scenePath})");
        }
        Debug.Log("============================================");
    }
    
    // Método para ir a la selección de niveles
    public void Jugar()
    {
        Debug.Log($"Intentando cargar escena: '{escenaSeleccionNiveles}'");
        
        // Verificar si la escena existe
        if (Application.CanStreamedLevelBeLoaded(escenaSeleccionNiveles))
        {
            SceneManager.LoadScene(escenaSeleccionNiveles);
        }
        else
        {
            Debug.LogError($"¡ESCENA NO ENCONTRADA! '{escenaSeleccionNiveles}' no está en Build Settings o el nombre es incorrecto.");
            Debug.Log("Verifica el nombre en el Inspector y asegúrate de que la escena esté en Build Settings.");
            
            // Como fallback, intentar cargar la siguiente escena
            CargarSiguienteEscena();
        }
    }
    
    // Método alternativo para ir directamente al primer nivel
    public void JugarDirecto()
    {
        Debug.Log($"Intentando cargar primer nivel: '{escenaPrimerNivel}'");
        
        if (Application.CanStreamedLevelBeLoaded(escenaPrimerNivel))
        {
            SceneManager.LoadScene(escenaPrimerNivel);
        }
        else
        {
            Debug.LogError($"¡ESCENA NO ENCONTRADA! '{escenaPrimerNivel}' no está en Build Settings.");
        }
    }
    
    // Método para cargar una escena específica por nombre
    public void CargarEscena(string nombreEscena)
    {
        Debug.Log($"Intentando cargar escena: '{nombreEscena}'");
        
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"¡ESCENA NO ENCONTRADA! '{nombreEscena}' no está disponible.");
        }
    }
    
    // Método para cargar la siguiente escena por índice
    public void CargarSiguienteEscena()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscena = escenaActual + 1;
        
        Debug.Log($"Escena actual índice: {escenaActual}, intentando cargar índice: {siguienteEscena}");
        
        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogWarning("No hay más escenas disponibles en Build Settings");
        }
    }
    
    // Método para cargar por índice específico
    public void CargarEscenaPorIndice(int indice)
    {
        Debug.Log($"Intentando cargar escena con índice: {indice}");
        
        if (indice >= 0 && indice < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(indice);
        }
        else
        {
            Debug.LogError($"Índice de escena inválido: {indice}. Rango válido: 0-{SceneManager.sceneCountInBuildSettings - 1}");
        }
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    [Header("DEBUG - Botones de Prueba")]
    [Space(10)]
    public bool mostrarBotonesDebug = true; // Variable dummy para que funcionen los atributos
    
    // Botones de debug - puedes eliminarlos después
    public void DebugMostrarEscenas()
    {
        Start(); // Llamar de nuevo el debug
    }
    
    public void DebugCargarEscena1() { CargarEscenaPorIndice(1); }
    public void DebugCargarEscena2() { CargarEscenaPorIndice(2); }
    public void DebugCargarEscena3() { CargarEscenaPorIndice(3); }
}