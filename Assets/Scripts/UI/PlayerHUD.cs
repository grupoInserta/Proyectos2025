using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider healthSlider;          // Barra de vida
    public TMPro.TextMeshProUGUI healthText;
    public PlayerHealth playerHealth;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }

    public void UpdateHealthUI(int current, int max)
    {
        Debug.Log("Actualizo salud: " + current + "--" + max);
        if (healthSlider != null)
            healthSlider.value = (float)current / max;

        if (healthText != null)
            healthText.text = $"{current} / {max}";
    }
}
