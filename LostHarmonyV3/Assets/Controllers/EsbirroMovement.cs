using UnityEngine;

public class EsbirroMovement : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private Camera mainCamera;
    private bool isActive = true;

    public void Initialize(Vector2 direction, float speed, Transform playerTransform)
    {
        moveDirection = direction;
        moveSpeed = speed;
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Mover el esbirro
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Verificar si el esbirro salió de la pantalla (por la izquierda o abajo)
        if (IsOutOfScreen())
        {
            Debug.Log("Esbirro salió de la pantalla, destruyendo: " + gameObject.name);
            Destroy(gameObject);
        }
    }

    bool IsOutOfScreen()
    {
        if (mainCamera == null) return false;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        // Si está muy lejos de la pantalla (considerando que la cámara se mueve)
        float margin = 100f;
        return (screenPos.x < -margin || screenPos.x > Screen.width + margin ||
                screenPos.y < -margin || screenPos.y > Screen.height + margin);
    }

    public void StopMovement()
    {
        isActive = false;
    }
}