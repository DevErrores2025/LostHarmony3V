using UnityEngine;

public class SpawnerBolasElectricas : MonoBehaviour
{
    public GameObject bolaElectricaPrefab;
    public Transform[] puntosSpawn; // Define múltiples puntos de aparición
    public float tiempoMin = 1.5f;
    public float tiempoMax = 4f;

    void Start()
    {
        Invoke("GenerarBola", Random.Range(tiempoMin, tiempoMax));
    }

    void GenerarBola()
    {
        int indice = Random.Range(0, puntosSpawn.Length);
        Instantiate(bolaElectricaPrefab, puntosSpawn[indice].position, Quaternion.identity);

        Invoke("GenerarBola", Random.Range(tiempoMin, tiempoMax));
    }
}
