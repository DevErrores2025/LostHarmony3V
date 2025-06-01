using UnityEngine;

public class RayoElectrico : MonoBehaviour
{
    public float daño = 20f;
    public float velocidad = 5f;
    public float vidaUtil = 5f;

    private Transform objetivo;

    void Start()
    {
        objetivo = GameObject.FindGameObjectWithTag("Jugador")?.transform;

        if (objetivo != null)
        {
            Vector2 direccion = ((Vector2)objetivo.position - (Vector2)transform.position).normalized;
            GetComponent<Rigidbody2D>().linearVelocity = direccion * velocidad;
        }

        Destroy(gameObject, vidaUtil); // Destruye la bola despu?s de cierto tiempo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        JugadorVida jugador = collision.GetComponent<JugadorVida>();
        if (jugador != null)
        {
            jugador.TomarDaño(daño);
            Destroy(gameObject);
        }
    }
}
