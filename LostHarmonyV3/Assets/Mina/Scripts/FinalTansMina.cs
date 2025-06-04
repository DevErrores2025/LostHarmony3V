using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalTansMina : MonoBehaviour
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

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(delayBeforeNextScene);

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}