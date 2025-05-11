using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float speed = 0.1f; // Velocidad de movimiento
    private Vector3 startPosition;
    private float spriteWidth;

    void Start()
    {
        startPosition = transform.position;

        // Buscar SpriteRenderer en hijos
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            spriteWidth = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("No se encontró SpriteRenderer en hijos de " + gameObject.name);
        }
    }

    void Update()
    {
        if (spriteWidth == 0) return; // Evitar división por cero

        float newPosition = Mathf.Repeat(Time.time * speed, spriteWidth);
        transform.position = startPosition + Vector3.left * newPosition;
    }
}
