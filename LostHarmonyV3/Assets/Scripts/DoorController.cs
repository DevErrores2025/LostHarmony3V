using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openHeight = 3f;
    public float openSpeed = 2f;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            StopAllCoroutines(); // Detener cualquier movimiento previo
            StartCoroutine(MoveDoor(openPosition));
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            StopAllCoroutines(); // Detener cualquier movimiento previo
            StartCoroutine(MoveDoor(closedPosition));
        }
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // Asegura que quede exactamente en la posición final
    }
}
