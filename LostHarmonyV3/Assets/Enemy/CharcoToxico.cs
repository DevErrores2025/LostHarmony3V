using UnityEngine;

public class CharcoToxico : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player tocó el charco tóxico");
            // Aquí asumes que el jugador tiene un script con una función TakeDamage()
            other.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
        }
    }
}
