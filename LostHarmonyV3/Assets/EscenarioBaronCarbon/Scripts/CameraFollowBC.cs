using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);
    
    [Header("Camera Bounds (Optional)")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    
    void Start()
    {
        // Buscar el player automáticamente si no está asignado
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
                player = GameObject.Find("Player");
            
            if (player != null)
                target = player.transform;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Posición deseada
        Vector3 desiredPosition = target.position + offset;
        
        // Aplicar límites si están habilitados
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Suavizar el movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}