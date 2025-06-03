using UnityEngine;

public class VikthorController : MonoBehaviour
{
    public Animator animator;
    public float tiempoEntreAtaques = 5f;

    private float temporizador;

    void Start()
    {
        temporizador = tiempoEntreAtaques;
    }

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            animator.SetTrigger("Golpea");
            temporizador = tiempoEntreAtaques;
        }   
    }
}
