using UnityEngine;
using UnityEngine.UI;

public class HealthBarAlc : MonoBehaviour
{
    public Image fillImage; // Asigna aquí el objeto HealthFill

    public void SetMaxHealth(int health)
    {
        fillImage.fillAmount = 1f;
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        fillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity; // Evita que la barra rote con el jugador
    }
}
