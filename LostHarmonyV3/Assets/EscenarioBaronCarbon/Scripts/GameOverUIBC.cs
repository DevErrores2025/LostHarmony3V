using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    private bool gameOver = false;
    
    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    // CAMBIADO: Ahora usa OnTriggerEnter2D porque los enemigos tienen trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !gameOver)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        gameOver = true;
        Time.timeScale = 0; // Pausar el juego
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1; // Reanudar tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}