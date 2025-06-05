using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelControllerLH : MonoBehaviour
{
    [Header("Botones de Niveles")]
    public Button buttonNivel1;
    public Button buttonNivel2;
    public Button buttonNivel3;
    public Button buttonNivel4;

    [Header("Botón Atrás")]
    public Button buttonAtras;

    [Header("Imágenes de Bloqueo (Opcional)")]
    public GameObject[] lockIcons; // Para mostrar candados en niveles bloqueados

    [Header("Nombres de Escenas")]
    public string escenaMenuPrincipal = "InicioDP";
    public string escenaNivel1 = "Escena DP";
    public string escenaNivel2 = "MainCombat";
    public string escenaNivel3 = "Alcantarillas";
    public string escenaNivel4 = "Escenario_DrVikthor2"; // Para futuro uso

    private Button[] todosLosBotones;

    void Start()
    {
        // Inicializar array de botones
        todosLosBotones = new Button[] { buttonNivel1, buttonNivel2, buttonNivel3, buttonNivel4};

        // Configurar los botones
        ConfigurarBotones();

        // Verificar qué niveles están desbloqueados
        ActualizarNivelesDesbloqueados();
    }

    void ConfigurarBotones()
    {
        // Configurar eventos de los botones de niveles
        if (buttonNivel1 != null) buttonNivel1.onClick.AddListener(() => CargarNivel(escenaNivel1));
        if (buttonNivel2 != null) buttonNivel2.onClick.AddListener(() => CargarNivel(escenaNivel2));
        if (buttonNivel3 != null) buttonNivel3.onClick.AddListener(() => CargarNivel(escenaNivel3));
        if (buttonNivel4 != null) buttonNivel4.onClick.AddListener(() => CargarNivel(escenaNivel4));

        // Configurar botón atrás
        if (buttonAtras != null) buttonAtras.onClick.AddListener(VolverAlMenuPrincipal);
    }

    void ActualizarNivelesDesbloqueados()
    {
        // Obtener el nivel más alto desbloqueado (por defecto solo el nivel 1)
        int nivelMaximoDesbloqueado = PlayerPrefs.GetInt("NivelMaximoDesbloqueado", 1);

        Debug.Log($"Nivel máximo desbloqueado: {nivelMaximoDesbloqueado}");

        // Actualizar cada botón según el progreso
        for (int i = 0; i < todosLosBotones.Length; i++)
        {
            if (todosLosBotones[i] != null)
            {
                int numeroNivel = i + 1;
                bool estaDesbloqueado = numeroNivel <= nivelMaximoDesbloqueado;

                // Activar/desactivar interacción del botón
                todosLosBotones[i].interactable = estaDesbloqueado;

                // Cambiar apariencia visual (opcional)
                CambiarAparienciaBoton(todosLosBotones[i], estaDesbloqueado);

                // Mostrar/ocultar candados (si los tienes)
                if (lockIcons != null && i < lockIcons.Length && lockIcons[i] != null)
                {
                    lockIcons[i].SetActive(!estaDesbloqueado);
                }
            }
        }
    }

    void CambiarAparienciaBoton(Button boton, bool desbloqueado)
    {
        // Cambiar el alpha del botón para indicar si está bloqueado
        Color colorBoton = boton.image.color;
        colorBoton.a = desbloqueado ? 1f : 0.5f;
        boton.image.color = colorBoton;
    }

    public void CargarNivel(string nombreEscena)
    {
        Debug.Log($"Cargando nivel: {nombreEscena}");
        SceneManager.LoadScene(nombreEscena);
    }

    public void VolverAlMenuPrincipal()
    {
        Debug.Log("Volviendo al menú principal");
        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    // Método para desbloquear el siguiente nivel (llamar desde otros scripts)
    public static void DesbloquearSiguienteNivel(int nivelCompletado)
    {
        int nivelActual = PlayerPrefs.GetInt("NivelMaximoDesbloqueado", 1);
        int siguienteNivel = nivelCompletado + 1;

        if (siguienteNivel > nivelActual && siguienteNivel <= 4) // Cambiado a 4 niveles
        {
            PlayerPrefs.SetInt("NivelMaximoDesbloqueado", siguienteNivel);
            PlayerPrefs.Save();
            Debug.Log($"¡Nivel {siguienteNivel} desbloqueado!");
        }
    }

    // Método para resetear progreso (útil para testing)
    [ContextMenu("Resetear Progreso")]
    public void ResetearProgreso()
    {
        PlayerPrefs.DeleteKey("NivelMaximoDesbloqueado");
        PlayerPrefs.Save();
        ActualizarNivelesDesbloqueados();
        Debug.Log("Progreso reseteado - Solo nivel 1 disponible");
    }
}