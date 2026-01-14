using UnityEngine;
using System;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public PlayerHUD miPlayerHUD;

    // Eventos para comunicar cambios a UI, inventario, GameCore...
    public event Action<int, int> OnHealthChanged;     // (current, max)
    public event Action OnPlayerDeath;
    public event Action<int> OnPlayerDamaged;


    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        NotifyHealthChange();
    }

    // Llamar cuando recibe daño
    public void TakeDamage(int amount)
    {   
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;

        // Avisar a GameCore o a la UI

        OnPlayerDamaged?.Invoke(currentHealth);//???
        miPlayerHUD.UpdateHealthUI(currentHealth, maxHealth);
        if (currentHealth <= 0)
            Die();
    }

    // Llamar desde objetos de curación del inventario
    // en principio no se van a hacer hobjetos de uracion
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        NotifyHealthChange();
    }

    private void Die()
    {
        isDead = true;
        OnPlayerDeath?.Invoke();
        Debug.Log("Jugador ha muerto.");
    }

    private void NotifyHealthChange()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log("salud actual: "+ currentHealth);
    }
}
