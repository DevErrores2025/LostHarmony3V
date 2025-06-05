using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelDP : MonoBehaviour
{
    public GameObject gameNextPanel;

    public void MostrarGameNext()
    {
        gameNextPanel.SetActive(true);
    }
    public void NextLevel()
    {
        // Preparar para el cambio de escena
        Time.timeScale = 1f;
        SceneManager.sceneLoaded += OnNextSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Pasando al siguiente nivel");
    }

    private void OnNextSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnNextSceneLoaded;

        // Configuraciones para la nueva escena
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Nueva escena cargada y configurada");
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("InicioDP");
    }
}
