using UnityEngine;

public class DanoAlJugador : MonoBehaviour
{
    public float cantidadDeDanio = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        JugadorVida jugador = collision.GetComponent<JugadorVida>();
        if (jugador != null)
        {
            jugador.TomarDaño(cantidadDeDanio);
            Destroy(gameObject); // Opcional: destruye la bola después de hacer daño
        }
    }
}
