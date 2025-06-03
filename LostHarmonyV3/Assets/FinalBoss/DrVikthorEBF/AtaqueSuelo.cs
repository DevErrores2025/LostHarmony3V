using System.Collections;
using UnityEngine;

public class AtaqueSuelo : MonoBehaviour
{
    public GameObject bolaPrefab;
    public Transform[] puntosSpawn;
    public GameObject cartelPrefab;
    public Transform posicionCartel;

    private GameObject cartelInstancia;

    public void GenerarAtaqueEspecial()
    {
        // Instanciar bolas
        foreach (Transform punto in puntosSpawn)
        {
            Instantiate(bolaPrefab, punto.position, Quaternion.identity);
        }

        // Instanciar el cartel
        cartelInstancia = Instantiate(cartelPrefab, posicionCartel.position, Quaternion.identity);

        // Iniciar la espera para destruir el cartel
        StartCoroutine(EsperarYDestruirCartel());
    }

    IEnumerator EsperarYDestruirCartel()
    {
        while (GameObject.FindObjectsOfType<BolaElectrica>().Length > 0)
        {
            yield return null; // Espera un frame
        }

        // Ya no hay bolas eléctricas, destruir cartel
        Destroy(cartelInstancia);
    }
}
