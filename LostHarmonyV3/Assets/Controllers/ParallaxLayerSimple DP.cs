using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayerSimple : MonoBehaviour
{
    [Header("Configuración")]
    public float parallaxFactor = 0.5f;

    [Header("Configuración Avanzada")]
    [Tooltip("Número de copias de la imagen (mínimo 3 recomendado)")]
    public int numberOfCopies = 3;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private Camera mainCamera;
    private float textureWidth;
    private List<Transform> copies = new List<Transform>();
    private Vector3 lastCameraPosition;
    private SpriteRenderer originalRenderer;
    private bool isControlledByBackground = false;

    void Start()
    {
        // Verificar si está siendo controlado por ParallaxBackground
        ParallaxBackground backgroundController = GetComponentInParent<ParallaxBackground>();
        isControlledByBackground = (backgroundController != null);

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            lastCameraPosition = mainCamera.transform.position;
        }

        originalRenderer = GetComponent<SpriteRenderer>();

        CalculateTextureWidth();
        CreateCopies();
        ArrangeInitialPositions();

        if (showDebugInfo)
        {
            Debug.Log($"{name}: Created {copies.Count} copies, texture width: {textureWidth}, controlled by background: {isControlledByBackground}");
        }
    }

    void CalculateTextureWidth()
    {
        if (originalRenderer != null && originalRenderer.sprite != null)
        {
            // Calcular el ancho real considerando bounds y escala
            textureWidth = originalRenderer.bounds.size.x;
        }
        else
        {
            textureWidth = 10f;
            Debug.LogWarning($"{name}: No se pudo calcular el ancho de la textura, usando valor por defecto");
        }
    }

    void CreateCopies()
    {
        // Añadir el objeto original a la lista
        copies.Add(transform);

        // Crear copias adicionales
        for (int i = 1; i < numberOfCopies; i++)
        {
            GameObject copy = Instantiate(gameObject, transform.parent);
            copy.name = $"{name}_Copy_{i}";

            // Remover este script de las copias para evitar recursión
            ParallaxLayerSimple copyScript = copy.GetComponent<ParallaxLayerSimple>();
            if (copyScript != null)
            {
                DestroyImmediate(copyScript);
            }

            copies.Add(copy.transform);
        }
    }

    void ArrangeInitialPositions()
    {
        // Posicionar las copias una al lado de la otra
        for (int i = 0; i < copies.Count; i++)
        {
            float xOffset = i * textureWidth;
            Vector3 newPos = copies[i].position;
            newPos.x = transform.position.x + xOffset;
            copies[i].position = newPos;
        }
    }

    void Update()
    {
        // Solo manejar el movimiento automáticamente si NO está controlado por ParallaxBackground
        if (!isControlledByBackground && mainCamera != null)
        {
            HandleAutomaticMovement();
        }
    }

    void HandleAutomaticMovement()
    {
        Vector3 deltaMovement = mainCamera.transform.position - lastCameraPosition;
        Move(deltaMovement.x, deltaMovement.y);
        lastCameraPosition = mainCamera.transform.position;
    }

    // Método público para ser llamado desde ParallaxBackground
    public void Move(float deltaX, float deltaY)
    {
        if (mainCamera == null) return;

        // Aplicar movimiento de parallax
        Vector3 parallaxMovement = new Vector3(deltaX * parallaxFactor, deltaY * parallaxFactor, 0);

        foreach (Transform copy in copies)
        {
            copy.position -= parallaxMovement;
        }

        CheckAndRepositionCopies();
    }

    void CheckAndRepositionCopies()
    {
        if (mainCamera == null) return;

        float cameraX = mainCamera.transform.position.x;
        float leftBound = cameraX - textureWidth * 2;  // Límite izquierdo (fuera de vista)
        float rightBound = cameraX + textureWidth * 2; // Límite derecho (fuera de vista)

        foreach (Transform copy in copies)
        {
            // Si una copia se fue muy a la izquierda, moverla al extremo derecho
            if (copy.position.x < leftBound)
            {
                float rightmostX = GetRightmostCopyX();
                copy.position = new Vector3(rightmostX + textureWidth, copy.position.y, copy.position.z);

                if (showDebugInfo)
                {
                    Debug.Log($"{copy.name}: Moved to right, new X: {copy.position.x}");
                }
            }
            // Si una copia se fue muy a la derecha, moverla al extremo izquierdo
            else if (copy.position.x > rightBound)
            {
                float leftmostX = GetLeftmostCopyX();
                copy.position = new Vector3(leftmostX - textureWidth, copy.position.y, copy.position.z);

                if (showDebugInfo)
                {
                    Debug.Log($"{copy.name}: Moved to left, new X: {copy.position.x}");
                }
            }
        }
    }

    float GetRightmostCopyX()
    {
        float rightmostX = float.MinValue;
        foreach (Transform copy in copies)
        {
            if (copy.position.x > rightmostX)
            {
                rightmostX = copy.position.x;
            }
        }
        return rightmostX;
    }

    float GetLeftmostCopyX()
    {
        float leftmostX = float.MaxValue;
        foreach (Transform copy in copies)
        {
            if (copy.position.x < leftmostX)
            {
                leftmostX = copy.position.x;
            }
        }
        return leftmostX;
    }

    void OnDestroy()
    {
        // Limpiar las copias creadas al destruir el objeto original
        for (int i = 1; i < copies.Count; i++)
        {
            if (copies[i] != null && copies[i] != transform)
            {
                if (Application.isPlaying)
                {
                    Destroy(copies[i].gameObject);
                }
                else
                {
                    DestroyImmediate(copies[i].gameObject);
                }
            }
        }
    }

    // Método para debugging - ver las posiciones de todas las copias
    [ContextMenu("Debug Copy Positions")]
    void DebugCopyPositions()
    {
        Debug.Log($"=== {name} Copy Positions ===");
        for (int i = 0; i < copies.Count; i++)
        {
            Debug.Log($"Copy {i}: X = {copies[i].position.x}");
        }
    }
}