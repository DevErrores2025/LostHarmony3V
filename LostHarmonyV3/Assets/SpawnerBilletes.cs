using UnityEngine;

public class SpawnerBilletes : MonoBehaviour
{
    public GameObject prefabBillete;
    public float intervaloGeneracion = 3f;
    public int maxBilletes = 10;

    public Vector2 areaMinima;
    public Vector2 areaMaxima;

    private int billetesActuales = 0;

    void Start()
    {
        InvokeRepeating(nameof(GenerarBillete), 1f, intervaloGeneracion);
    }

    void GenerarBillete()
    {
        if (billetesActuales >= maxBilletes) return;

        Vector2 posicionAleatoria = new Vector2(
            Random.Range(areaMinima.x, areaMaxima.x),
            -2f // Suponiendo que el piso está en Y = -2
        );

        Instantiate(prefabBillete, posicionAleatoria, Quaternion.identity);
        billetesActuales++;
    }

    public void BilleteRecolectado()
    {
        billetesActuales--;
    }
}
