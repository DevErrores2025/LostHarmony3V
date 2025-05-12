using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform objetivo; // Asignarás el submarino aquí desde el editor
    public float suavizado = 0.125f; // Qué tan suavemente sigue la cámara al submarino
    public Vector3 offset = new Vector3(0, 0, -10); // Distancia de la cámara al submarino

    void LateUpdate()
    {
        // Si no hay objetivo, no hacer nada
        if (objetivo == null)
            return;

        // Calcular la posición deseada
        Vector3 posicionDeseada = objetivo.position + offset;

        // Suavizar el movimiento
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado);

        // Aplicar solo el movimiento en X y Y, manteniendo Z constante
        transform.position = new Vector3(posicionSuavizada.x, posicionSuavizada.y, offset.z);
    }
}