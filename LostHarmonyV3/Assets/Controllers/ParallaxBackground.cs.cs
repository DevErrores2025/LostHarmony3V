using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    public ParallaxCamera parallaxCamera;
    public bool moverEnY = false; // Para permitir parallax vertical

    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void Start()
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
            parallaxCamera.onCameraTranslate += Move;
        }
        else
        {
            Debug.LogWarning("No se encontró ParallaxCamera. Asegúrate de que la cámara principal tenga el componente ParallaxCamera.");
        }

        SetLayers();
    }

    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);
                Debug.Log($"Agregado layer: {layer.name} con factor: {layer.parallaxFactor}");
            }
        }

        Debug.Log($"Total de layers de parallax: {parallaxLayers.Count}");
    }

    void Move(float deltaX, float deltaY)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
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

    void OnValidate()
    {
        // Actualizar layers cuando se cambien valores en el inspector
        if (Application.isPlaying)
        {
            SetLayers();
        }
    }
}