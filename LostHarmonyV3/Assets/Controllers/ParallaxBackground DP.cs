using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    public ParallaxCamera parallaxCamera;
    public bool moverEnY = false; // Para permitir parallax vertical

    private List<ParallaxLayerSimple> parallaxLayers = new List<ParallaxLayerSimple>();
    private Vector3 lastCameraPosition;

    void Start()
    {
        InitializeParallaxCamera();
        SetLayers();

        // Guardar posición inicial de la cámara
        if (parallaxCamera != null && parallaxCamera.transform != null)
        {
            lastCameraPosition = parallaxCamera.transform.position;
        }
        else if (Camera.main != null)
        {
            lastCameraPosition = Camera.main.transform.position;
        }
    }

    void InitializeParallaxCamera()
    {
        if (parallaxCamera == null)
        {
            // Buscar ParallaxCamera en la cámara principal
            if (Camera.main != null)
            {
                parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
            }
        }

        if (parallaxCamera != null)
        {
            // Suscribirse al evento si existe
            if (parallaxCamera.onCameraTranslate != null)
            {
                parallaxCamera.onCameraTranslate += Move;
            }
            else
            {
                Debug.LogWarning("ParallaxCamera no tiene el evento onCameraTranslate configurado. Usando Update() como fallback.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ParallaxCamera. El sistema funcionará usando Camera.main como fallback.");
        }
    }

    void SetLayers()
    {
        parallaxLayers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayerSimple layer = transform.GetChild(i).GetComponent<ParallaxLayerSimple>();
            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);
                Debug.Log($"Agregado layer: {layer.name} con factor: {layer.parallaxFactor}");
            }
        }
        Debug.Log($"Total de layers de parallax: {parallaxLayers.Count}");
    }

    void Update()
    {
        // Si no tenemos ParallaxCamera o su evento, manejar el movimiento manualmente
        if (parallaxCamera == null || parallaxCamera.onCameraTranslate == null)
        {
            HandleFallbackMovement();
        }
    }

    void HandleFallbackMovement()
    {
        Camera activeCamera = parallaxCamera != null ? parallaxCamera.GetComponent<Camera>() : Camera.main;

        if (activeCamera != null)
        {
            Vector3 currentCameraPosition = activeCamera.transform.position;
            Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;

            if (deltaMovement.magnitude > 0.001f) // Solo mover si hay cambio significativo
            {
                Move(deltaMovement.x, deltaMovement.y);
            }

            lastCameraPosition = currentCameraPosition;
        }
    }

    void Move(float deltaX, float deltaY)
    {
        foreach (ParallaxLayerSimple layer in parallaxLayers)
        {
            if (layer != null)
            {
                if (moverEnY)
                {
                    layer.Move(deltaX, deltaY);
                }
                else
                {
                    layer.Move(deltaX, 0);
                }
            }
        }
    }

    void OnValidate()
    {
        // Actualizar layers cuando se cambien valores en el inspector
        if (Application.isPlaying)
        {
            SetLayers();
        }
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar errores
        if (parallaxCamera != null && parallaxCamera.onCameraTranslate != null)
        {
            parallaxCamera.onCameraTranslate -= Move;
        }
    }

    // Método para debugging
    [ContextMenu("Debug Parallax Layers")]
    void DebugParallaxLayers()
    {
        Debug.Log($"=== {name} Parallax Layers ===");
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            if (parallaxLayers[i] != null)
            {
                Debug.Log($"Layer {i}: {parallaxLayers[i].name}, Factor: {parallaxLayers[i].parallaxFactor}");
            }
        }
    }
}