using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;

    public void MostrarGameOver() { 
        gameOverPanel.SetActive(true);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene("InicioDP");
    }
}
