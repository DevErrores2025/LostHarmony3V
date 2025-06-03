using System.Collections;
using UnityEngine;

public class PlayerHealthAlc : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBarAlc healthBar; // Arrastra el objeto de UI aquí
    private Animator animator;
    private bool invulnerable = false;
    public float invulnerableDuration = 0.5f;
    private SpriteRenderer[] spriteRenderers;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("Danio"); // Lanza la animación de daño
            StartCoroutine(FlashRed());
            StartCoroutine(TemporarilyInvulnerable());
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died");
        SceneController.instance.ShowGameOver();
    }

    private IEnumerator TemporarilyInvulnerable()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
    }

    private IEnumerator FlashRed()
    {
        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.red;
        }

        yield return new WaitForSeconds(0.15f);

        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.white;
        }
    }
}

