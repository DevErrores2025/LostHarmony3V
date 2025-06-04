using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyKill : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    
    void Start()
    {
        // Buscar el panel de Game Over en la escena
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        Time.timeScale = 0; // Pausar juego
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}