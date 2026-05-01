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

    private CambioEscena cambioEscena;

    private bool isDead = false;
    // sonidos
    public AudioSource audioSource;
    public AudioClip PasosSound;
    public AudioClip SprintSound;
    public AudioClip CrouchSound;
    private AudioClip targetClip;
    private FirstPersonController firstPersonController;

    private void Awake()
    {
        currentHealth = maxHealth;
        cambioEscena = GetComponent<CambioEscena>();
        firstPersonController = gameObject.GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        NotifyHealthChange();
        audioSource.loop = true;
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
        cambioEscena.CargaEscena("Derrota");
        Debug.Log("Jugador ha muerto.");
    }

    public void NotifyHealthChange()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log("salud actual: " + currentHealth);
    }

    void Update()
    {         
        if (firstPersonController.isWalking || firstPersonController.isSprinting)
        {
            AudioClip targetClip = null;
            if (firstPersonController.isWalking)
            {
                targetClip = PasosSound;
                Debug.Log("PASOS");
            }
            if(firstPersonController.isSprinting)
            {
                Debug.Log("CORRIENDO");
                targetClip = SprintSound;
            }
            if(firstPersonController.isCrouched && firstPersonController.isWalking)
            {
                targetClip = CrouchSound;
            }
           

            // Cambiar clip solo si es distinto (evita reinicios constantes)
            if (audioSource.clip != targetClip)
            {
                audioSource.clip = targetClip;
                audioSource.Play();
            }

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
       
    }


}
