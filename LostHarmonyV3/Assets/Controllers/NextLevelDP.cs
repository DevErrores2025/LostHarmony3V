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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Pasando al siguiente nivel");

    }
    public void IrAlMenu()
    {
        SceneManager.LoadScene("InicioDP");
    }
}
