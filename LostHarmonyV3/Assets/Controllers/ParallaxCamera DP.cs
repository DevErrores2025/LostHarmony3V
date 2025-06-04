using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaX, float deltaY);
    public ParallaxCameraDelegate onCameraTranslate;

    private Vector3 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        if (currentPosition != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                float deltaX = oldPosition.x - currentPosition.x;
                float deltaY = oldPosition.y - currentPosition.y;
                onCameraTranslate(deltaX, deltaY);
            }
            oldPosition = currentPosition;
        }
    }
}
