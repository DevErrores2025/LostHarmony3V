using UnityEngine;
using TMPro;

public class GameManagerDP : MonoBehaviour
{
    public static GameManagerDP Instance;

    [Header("Configuración UI")]
    public TextMeshProUGUI basuraEsquivadaText;
    public TextMeshProUGUI puntuacionText;

    [Header("Fin del Juego")]
    public GameObject panelFinJuego; // Panel opcional para mostrar al finalizar
    public string mensajeFinal = "¡Has esquivado toda la basura!";

    private int _esbirrosEsquivados = 0;
    private int _puntuacion = 0;

    private const int ObjetivoEsquivar = 10;
    private bool juegoFinalizado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ActualizarUI();
    }

    public void AddScore(int puntos)
    {
        if (juegoFinalizado) return;

        _puntuacion += puntos;
        ActualizarUI();
        Debug.Log($"Puntuación añadida: {puntos}. Total: {_puntuacion}");
    }

    public void EsbirroEsquivado()
    {
        if (juegoFinalizado) return;

        _esbirrosEsquivados++;
        ActualizarUI();

        if (_esbirrosEsquivados >= ObjetivoEsquivar)
        {
            Debug.Log("¡Objetivo de esquivar completado!");
            FinalizarJuego();
        }
    }

    void ActualizarUI()
    {
        if (basuraEsquivadaText != null)
        {
            basuraEsquivadaText.text = $"{_esbirrosEsquivados}";
        }

        if (puntuacionText != null)
        {
            puntuacionText.text = $"PUNTOS: {_puntuacion}";
        }
    }

    void FinalizarJuego()
    {
        juegoFinalizado = true;

        // Detener el tiempo si deseas
        Time.timeScale = 0f;

        // Mostrar panel o mensaje final
        if (panelFinJuego != null)
        {
            panelFinJuego.SetActive(true);
        }

        Debug.Log(mensajeFinal);
    }
}
