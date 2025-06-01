using UnityEngine;
using System.Collections;

public class DamaPlasticaManager : MonoBehaviour
{
    [Header("Configuración Aparición")]
    public float tiempoAparicion = 1.5f;
    public Vector3 posicionFinal = new Vector3(8f, 2f, 0f); // Posición donde se posiciona la dama
    public Vector3 posicionInicial = new Vector3(15f, 2f, 0f); // Fuera de pantalla

    [Header("Configuración Animación")]
    public Animator animator;

    private bool estaActiva = true; // Cambiado a true por defecto
    private bool estaLanzando = false;
    private int esbirrosLanzados = 0;

    [Header("Persecución al Jugador")]
    public Transform jugador;           // Referencia al jugador
    public float velocidadPersecucion = 2f; // Velocidad con la que sigue al jugador

    [Header("Movimiento Horizontal")]
    public bool seguirHorizontalmente = true;
    public float offsetHorizontal = 2f; // Distancia horizontal para mantener del jugador
    public float suavizadoMovimiento = 5f;

    [Header("Movimiento Avanzado")]
    public float amplitudOndulacion = 0.5f; // Movimiento ondulante vertical
    public float frecuenciaOndulacion = 1f;
    public float velocidadAjustePosicion = 3f;

    private float tiempoOndulacion;

    void Start()
    {
    }

    void Update()
    {
        if (jugador != null)
        {
            Vector3 direccion = (jugador.position - transform.position).normalized;
            transform.position += direccion * velocidadPersecucion*Time.deltaTime;

        }
    }

}