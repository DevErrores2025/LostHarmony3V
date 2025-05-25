using UnityEngine;

[System.Serializable]
public class ParallaxLayerData
{
    public Transform layer;
    [Range(0f, 1f)] public float multiplier = 0.5f;
}

public class ParallaxController : MonoBehaviour
{
    public Transform player; 
    public ParallaxLayerData[] layers;

    private float lastPlayerX;

    void Start()
    {
        lastPlayerX = player.position.x;
    }

    void LateUpdate()
    {
        float deltaX = player.position.x - lastPlayerX;

        if (Mathf.Abs(deltaX) > 0.0001f)
        {
            foreach (var layerData in layers)
            {
                if (layerData.layer != null)
                {
                    Vector3 pos = layerData.layer.position;
                    pos.x += deltaX * layerData.multiplier;
                    pos.x = Mathf.Round(pos.x * 100f) / 100f; 
                    layerData.layer.position = pos;
                }
            }
        }

        lastPlayerX = player.position.x;
    }
}

