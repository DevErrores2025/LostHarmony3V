using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RecoleccionBilletes : MonoBehaviour
{
    public int billetesRecolectados = 0;
    public int billetesParaGanar = 10;
    public TextMeshProUGUI textoBilletes;
    public GameObject panelGanaste;

    void Start()
    {
        ActualizarTexto();
        if (panelGanaste != null)
            panelGanaste.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Billete"))
        {
            billetesRecolectados++;
            Destroy(collision.gameObject);
            ActualizarTexto();

            if (billetesRecolectados >= billetesParaGanar)
            {
                GanarJuego();
            }
        }
    }

    void ActualizarTexto()
    {
        if (textoBilletes != null)
            textoBilletes.text = " " + billetesRecolectados + " / " + billetesParaGanar;
    }

    void GanarJuego()
    {
        Time.timeScale = 0f; // Pausa el juego
        if (panelGanaste != null)
            panelGanaste.SetActive(true);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Escenario_DrVikthor2"); // Cambia por el nombre real de tu menú
    }
}
