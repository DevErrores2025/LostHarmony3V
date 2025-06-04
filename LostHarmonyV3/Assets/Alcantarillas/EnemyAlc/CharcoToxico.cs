using UnityEngine;

public class CharcoToxico : MonoBehaviour
{
    public int damageAmount = 10;
    public float repulsionForce = 8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player tocó el charco tóxico");
            // Aquí asumes que el jugador tiene un script con una función TakeDamage()
            other.GetComponent<PlayerHealthAlc>()?.TakeDamage(damageAmount);

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 repulsionDirection = (other.transform.position - transform.position).normalized;
                rb.linearVelocity = Vector2.zero; // opcional: para reiniciar su velocidad actual
                rb.AddForce(repulsionDirection * repulsionForce, ForceMode2D.Impulse);
            }
        }
    }
}
