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

    void Start()
    {
        lastCamPosition = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = Camera.main.transform.position - lastCamPosition;

        foreach (var layerData in layers)
        {
            if (layerData.layer != null)
            {
                layerData.layer.position += new Vector3(delta.x * layerData.multiplier, 0f, 0f);
            }
        }

        lastCamPosition = Camera.main.transform.position;
    }
}

