using UnityEngine;

public class SkullProjectile : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger hit: " + other.name);
            other.GetComponent<PlayerHealth>()?.TakeDamage(5);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
