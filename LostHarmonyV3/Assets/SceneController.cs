using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("UI Game Over")]
    public GameObject gameOverUI; // arrastra el panel con botones
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
         }
        else
        { 
            Destroy(gameObject);
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f; // pausa el juego
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AssignGameOverUIWithDelay());
    }

    IEnumerator AssignGameOverUIWithDelay()
    {
        yield return null;
        yield return null;

        if (gameOverUI == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "GameOverUI")
                {
                    gameOverUI = obj;
                    break;
                }
            }
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
            Debug.Log("✅ GameOverUI encontrado y asignado incluso estando desactivado.");
        }
        else
        {
            Debug.LogWarning("❌ GameOverUI sigue sin encontrarse incluso con búsqueda extendida.");
        }

        isGameOver = false;
    }


    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
