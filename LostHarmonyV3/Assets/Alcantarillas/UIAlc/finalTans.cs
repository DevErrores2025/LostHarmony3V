using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finalTans : MonoBehaviour
{
    public GameObject transitionImage; 
    public float delayBeforeNextScene = 3f;
    public string nextSceneName = "Scenes/Personaje"; // O "Nivel2", según tu flujo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowEndImage());
        }
    }

    IEnumerator ShowEndImage()
    {
        Debug.Log("Activando imagen final");
        Time.timeScale = 0f; 
        transitionImage.SetActive(true); 

        yield return new WaitForSecondsRealtime(delayBeforeNextScene);

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}
