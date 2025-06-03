using System.Collections;
using UnityEngine;

public class LightningSpawner2 : MonoBehaviour
{
    public GameObject lightningPrefab;
    public Transform[] spawnPoints;
    public float[] lightningDurations; // Duración individual de cada rayo
    public float delayBetweenSpawns = 0.4f;

    void Start()
    {
        StartCoroutine(SpawnLightningSequence());
    }

    IEnumerator SpawnLightningSequence()
    {
        while (true)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                GameObject lightning = Instantiate(lightningPrefab, spawnPoints[i].position, Quaternion.identity);

                // Asegura que haya una duración definida para cada rayo
                float duration = (i < lightningDurations.Length) ? lightningDurations[i] : 0.5f;
                Destroy(lightning, duration);

                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            yield return new WaitForSeconds(2f); // Espera antes de lanzar otra secuencia de rayos
        }
    }
}
