using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject door; // Asigna la puerta desde el inspector
    public Color pressedColor = Color.green;
    public float sinkDistance = 0.2f; // Distancia que se hunde
    public float sinkSpeed = 2f; // Velocidad al hundirse

    private Vector3 initialPosition;
    private bool isPressed = false;
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        initialPosition = transform.position;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color; // Guardar el color original
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Objeto entró al trigger: " + other.name);

        if (!isPressed)
        {
            isPressed = true;
            rend.material.color = pressedColor; // Cambiar a verde
            StartCoroutine(SinkButton());
            door.GetComponent<DoorController>().OpenDoor(); // Llama a la puerta
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Objeto salió del trigger: " + other.name);

        if (isPressed)
        {
            isPressed = false;
            rend.material.color = originalColor; // Restaurar color original
            StartCoroutine(RiseButton());
            door.GetComponent<DoorController>().CloseDoor(); // Llama para cerrar la puerta
        }
    }

    private System.Collections.IEnumerator SinkButton()
    {
        Vector3 targetPosition = initialPosition - new Vector3(0, sinkDistance, 0);
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, sinkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private System.Collections.IEnumerator RiseButton()
    {
        Vector3 targetPosition = initialPosition;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, sinkSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
