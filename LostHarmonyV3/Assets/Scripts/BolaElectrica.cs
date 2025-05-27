using UnityEngine;

public class BolaElectrica : MonoBehaviour
{
    public float velocidad = 2f;
    public float alturaDesaparicion = 6f; // Ajusta según tu escena

    void Update()
    {
        // Mover hacia arriba
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);

        // Si sale de pantalla (por altura), se destruye
        if (transform.position.y >= alturaDesaparicion)
        {
            Destroy(gameObject);
        }
    }
}
