using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform objetivo;
    public float velocidadCamara = 0.025f;
    public Vector3 desplazamiento;

    [Header("Límites de Cámara")]
    public float limiteMaximoY = -1.44f;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        // Aplicar límite en Y - la cámara no puede ir más abajo de limiteMaximoY
        if (posicionDeseada.y < limiteMaximoY)
        {
            posicionDeseada.y = limiteMaximoY;
        }

        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);
        transform.position = posicionSuavizada;
    }
}