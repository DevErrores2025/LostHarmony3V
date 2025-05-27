using UnityEngine;

public class MoverRayo : MonoBehaviour
{
    public float velocidad = 10f;

    void Update()
    {
        transform.Translate(Vector3.right * velocidad * Time.deltaTime);
    }

    // Destruye el rayo después de cierto tiempo para que no se quede en escena
    void Start()
    {
        Destroy(gameObject, 3f);
    }
}
