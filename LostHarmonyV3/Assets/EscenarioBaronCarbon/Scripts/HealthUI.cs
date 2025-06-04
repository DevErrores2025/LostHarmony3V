using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Text healthText;
    public Image healthFill;
    
    [Header("Color Settings")]
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f;
    public float midHealthThreshold = 0.6f;
    
    [Header("Animation")]
    public bool animateHealthChange = true;
    public float animationSpeed = 2f;
    
    private float targetHealth = 1f;
    private float currentDisplayHealth = 1f;
    
    void Start()
    {
        // Find player health component
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            // Subscribe to health change events
            playerHealth.OnHealthChanged += UpdateHealthDisplay;
            
            // Initialize with current health
            UpdateHealthDisplay(playerHealth.GetHealthPercentage());
        }
        
        // Initialize UI
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        // Animate health changes
        if (animateHealthChange && Mathf.Abs(currentDisplayHealth - targetHealth) > 0.01f)
        {
            currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth, animationSpeed * Time.deltaTime);
            UpdateUI();
        }
    }
    
    public void UpdateHealthDisplay(float healthPercentage)
    {
        targetHealth = Mathf.Clamp01(healthPercentage);
        
        if (!animateHealthChange)
        {
            currentDisplayHealth = targetHealth;
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        // Update slider
        if (healthSlider != null)
        {
            healthSlider.value = currentDisplayHealth;
        }
        
        // Update text
        if (healthText != null)
        {
            float actualHealth = currentDisplayHealth * 100f;
            healthText.text = $"{actualHealth:F0}%";
        }
        
        // Update color based on health percentage
        UpdateHealthColor();
    }
    
    void UpdateHealthColor()
    {
        Color targetColor;
        
        if (currentDisplayHealth <= lowHealthThreshold)
        {
            targetColor = lowHealthColor;
        }
        else if (currentDisplayHealth <= midHealthThreshold)
        {
            // Interpolate between low and mid
            float t = (currentDisplayHealth - lowHealthThreshold) / (midHealthThreshold - lowHealthThreshold);
            targetColor = Color.Lerp(lowHealthColor, midHealthColor, t);
        }
        else
        {
            // Interpolate between mid and full
            float t = (currentDisplayHealth - midHealthThreshold) / (1f - midHealthThreshold);
            targetColor = Color.Lerp(midHealthColor, fullHealthColor, t);
        }
        
        // Apply color to health fill
        if (healthFill != null)
        {
            healthFill.color = targetColor;
        }
    }
}