using UnityEngine;

public class SpawnerRayos : MonoBehaviour
{
    public GameObject RayoElectricoPrefab;
    public Transform[] puntosSpawn;
    public float tiempoMin = 1.5f;
    public float tiempoMax = 4f;

    void Start()
    {
        Invoke("GenerarRayo", Random.Range(tiempoMin, tiempoMax));
    }

    void GenerarRayo()
    {
        if (puntosSpawn.Length == 0)
        {
            Debug.LogWarning("No hay puntos de spawn asignados.");
            return;
        }

        int indice = Random.Range(0, puntosSpawn.Length);
        Vector3 pos = puntosSpawn[indice].position;
        pos.z = 0;

        Debug.Log("Instanciando rayo en: " + pos);

        Instantiate(RayoElectricoPrefab, pos, Quaternion.identity);

        Invoke("GenerarRayo", Random.Range(tiempoMin, tiempoMax));
    }
}
