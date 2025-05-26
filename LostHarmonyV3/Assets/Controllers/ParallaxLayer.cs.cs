using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Header("Configuración Parallax")]
    public float parallaxFactor = 1f;

    [Header("Configuración Repetición")]
    public bool repetirHorizontalmente = true;
    public bool repetirVerticalmente = false;

    private float anchoTextura;
    private float altoTextura;
    private Vector3 posicionInicialCamara;
    private Transform camaraTransform;

    void Start()
    {
        // Obtener el tamaño de la textura/sprite
        CalcularTamañoTextura();

        // Obtener referencia a la cámara
        if (Camera.main != null)
        {
            camaraTransform = Camera.main.transform;
            posicionInicialCamara = camaraTransform.position;
        }
    }

    void CalcularTamañoTextura()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Calcular el ancho real considerando la escala
            anchoTextura = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
            altoTextura = spriteRenderer.sprite.bounds.size.y * transform.localScale.y;
        }
        else
        {
            // Valores por defecto si no hay sprite
            anchoTextura = 10f;
            altoTextura = 10f;
        }

        Debug.Log($"Layer {name}: Ancho={anchoTextura}, Alto={altoTextura}");
    }

    public void Move(float deltaX, float deltaY = 0)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= deltaX * parallaxFactor;
        newPos.y -= deltaY * parallaxFactor;
        transform.localPosition = newPos;

        // Verificar si necesita repetirse
        if (repetirHorizontalmente)
        {
            VerificarRepeticionHorizontal();
        }

        if (repetirVerticalmente)
        {
            VerificarRepeticionVertical();
        }
    }

    void VerificarRepeticionHorizontal()
    {
        if (camaraTransform == null) return;

        float distanciaCamara = camaraTransform.position.x - posicionInicialCamara.x;
        float distanciaLayer = distanciaCamara * parallaxFactor;

        // Si el layer se ha movido más de su ancho, reposicionarlo
        if (Mathf.Abs(distanciaLayer) >= anchoTextura)
        {
            float offset = Mathf.Sign(distanciaLayer) * anchoTextura;
            Vector3 pos = transform.localPosition;
            pos.x += offset;
            transform.localPosition = pos;

            // Actualizar la posición inicial de referencia
            posicionInicialCamara.x += offset / parallaxFactor;
        }
    }

    void VerificarRepeticionVertical()
    {
        if (camaraTransform == null) return;

        float distanciaCamara = camaraTransform.position.y - posicionInicialCamara.y;
        float distanciaLayer = distanciaCamara * parallaxFactor;

        if (Mathf.Abs(distanciaLayer) >= altoTextura)
        {
            float offset = Mathf.Sign(distanciaLayer) * altoTextura;
            Vector3 pos = transform.localPosition;
            pos.y += offset;
            transform.localPosition = pos;

            posicionInicialCamara.y += offset / parallaxFactor;
        }
    }
}
