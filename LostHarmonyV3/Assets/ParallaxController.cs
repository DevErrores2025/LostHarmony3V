using UnityEngine;

[System.Serializable]
public class ParallaxLayerData
{
    public Transform layer;
    [Range(0f, 1f)] public float multiplier = 0.5f;
}

public class ParallaxController : MonoBehaviour
{
    public ParallaxLayerData[] layers;

    private Vector3 lastCamPosition;
    private const float movimientoMinimo = 0.1f;

    void Start()
    {
        lastCamPosition = Camera.main.transform.position;

    }

    void LateUpdate()
    {
        Vector3 camPosition = Camera.main.transform.position;
        Vector3 delta = camPosition - lastCamPosition;

        if (Mathf.Abs(delta.x) > movimientoMinimo)
        {
            foreach (var layerData in layers)
            {
                if (layerData.layer != null)
                {
                    Vector3 newPos = layerData.layer.position + new Vector3(delta.x * layerData.multiplier, 0f, 0f);
                    newPos.x = Mathf.Round(newPos.x * 100f) / 100f; // Redondeo para evitar jitter
                    layerData.layer.position = newPos;
                }
            }
        }

        lastCamPosition = camPosition;
    }
}

