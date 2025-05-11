using UnityEngine;

public class VillanoDisparo : MonoBehaviour
{
    public GameObject prefabRayo;          // El rayo que se va a disparar
    public Transform puntoDisparo;         // Punto desde donde se dispara
    public float intervaloDisparo = 3f;    // Tiempo entre disparos

    private void Start()
    {
        InvokeRepeating(nameof(DispararRayo), 1f, intervaloDisparo);  // Comienza después de 1s y repite
    }

    void DispararRayo()
    {
        Instantiate(prefabRayo, puntoDisparo.position, puntoDisparo.rotation);
    }
}
