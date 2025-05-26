using UnityEngine;

public class EsbirroMovement : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    public float velocidadBase = 3f;
    public float velocidadRotacion = 50f;

    [Header("Configuración Patrón")]
    public bool movimientoOndulante = true;
    public float amplitudOndulacion = 1f;
    public float frecuenciaOndulacion = 2f;

    private float velocidadActual;
    private bool vieneDelAbanico = false;
    private Vector2 moveDirection;
    private Vector2 direccionMovimiento = Vector2.down;
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

    public void ConfigurarVelocidad(float nuevaVelocidad)
    {
        velocidadBase = nuevaVelocidad;
        velocidadActual = nuevaVelocidad;
    }
    void ConfigurarMovimientoSegunOrigen()
    {
        if (vieneDelAbanico)
        {
            // Los esbirros del abanico se mueven directamente hacia abajo/jugador
            direccionMovimiento = Vector2.down;
            movimientoOndulante = false; // Movimiento más directo desde el abanico
        }
        else
        {
            // Los esbirros normales pueden tener movimiento ondulante
            direccionMovimiento = Vector2.down;
        }
    }

    public void ConfigurarOrigenAbanico(bool esDelAbanico)
    {
        vieneDelAbanico = esDelAbanico;
        ConfigurarMovimientoSegunOrigen();

        if (vieneDelAbanico)
        {
            // Los esbirros del abanico pueden ser ligeramente más rápidos
            velocidadActual *= 1.2f;
        }
    }
}