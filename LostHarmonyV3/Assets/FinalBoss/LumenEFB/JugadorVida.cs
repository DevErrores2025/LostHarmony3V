using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class JugadorVida : MonoBehaviour
{
    public float vidaMaxima = 100f;
    private float vidaActual;
    public Image barraVida;
    public GameObject panelGameOver;
    public Animator animator;

    private SpriteRenderer[] spriteRenderers;
    private Color colorOriginal = Color.white;

    public bool estaMuerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarBarraVida();

        panelGameOver.SetActive(false);

        // ?? Obtener correctamente el SpriteRenderer del personaje
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers.Length > 0)
        {
            colorOriginal = spriteRenderers[0].color;
        }
        else
        {
            Debug.LogError("? No se encontraron SpriteRenderers. Asegúrate de que el personaje tenga uno o más.");
        }

    }

    public void TomarDaño(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        ActualizarBarraVida();

        // ? Cambiar color temporal a rojo al recibir daño
        StartCoroutine(CambiarColorTemporal(Color.red, 0.2f));

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            estaMuerto = true;

            // ? Reproducir animación de muerte y NO volver a otra animación
            animator.Play("AnimacionMuerteLumen");
            StartCoroutine(MostrarGameOverConRetraso());
        }
        else
        {
            animator.Play("AnimacionDanoLumen");
            StartCoroutine(VolverAIdle(0.5f));
        }
    }

    void ActualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = vidaActual / vidaMaxima;
        }
    }

    IEnumerator CambiarColorTemporal(Color colorTemporal, float duracion)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.color = colorTemporal;
        }

        yield return new WaitForSeconds(duracion);

        if (!estaMuerto)
        {
            foreach (var sr in spriteRenderers)
            {
                sr.color = colorOriginal;
            }
        }
    }


    IEnumerator VolverAIdle(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        if (!estaMuerto)
        {
            animator.Play("Idle");
        }
    }

    IEnumerator MostrarGameOverConRetraso()
    {
        yield return new WaitForSeconds(1.2f); // espera a que se vea la animación  

        panelGameOver.SetActive(true);

        // Desactivar inputs y movimiento
        Time.timeScale = 0f; // ?? Esto detiene todo el juego
    }

    // ? MÉTODOS PUBLICOS PARA UI

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlInicio()
    {
        SceneManager.LoadScene("Escenario_DrVikthor2"); // Asegúrate de que este nombre exista en File > Build Settings
    }
}
