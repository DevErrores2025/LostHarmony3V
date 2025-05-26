using UnityEngine;
using TMPro;

public class GameManagerDP : MonoBehaviour
{
    public static GameManagerDP Instance;

    [Header("Configuración UI")]
    public TextMeshProUGUI basuraEsquivadaText;
    public TextMeshProUGUI puntuacionText; // Nuevo texto para mostrar puntuación

    private int _esbirrosEsquivados = 0;
    private int _puntuacion = 0; // Nueva variable para puntuación
    private const int ObjetivoEsquivar = 10;

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

    // Método para añadir puntuación (que faltaba)
    public void AddScore(int puntos)
    {
        _puntuacion += puntos;
        ActualizarUI();
        Debug.Log($"Puntuación añadida: {puntos}. Total: {_puntuacion}");
    }

    public void EsbirroEsquivado()
    {
        _esbirrosEsquivados++;
        ActualizarUI();

        if (_esbirrosEsquivados >= ObjetivoEsquivar)
        {
            Debug.Log("¡Objetivo de esquivar completado!");
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
}