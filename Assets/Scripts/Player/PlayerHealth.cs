using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public PlayerHUD miPlayerHUD;
    public Image Rojo;
    //
    private float lastY;
    private float verticalSpeed;

    // Eventos para comunicar cambios a UI, inventario, GameCore...
    public event Action<int, int> OnHealthChanged;     // (current, max)
    public event Action OnPlayerDeath;
    public event Action<int> OnPlayerDamaged;
    [SerializeField] private int amountDamage;

    private CambioEscena cambioEscena;

    private bool isDead = false;
    // sonidos
    public AudioSource audioSource;
    public AudioSource audioSource4;
    public AudioClip PasosSound;
    public AudioClip Aterrizaje;
    public AudioClip SprintSound;
    public AudioClip CrouchSound;
    public AudioClip DamageSound;
    public AudioClip audioAgotado;
    public AudioClip CrouchAudio;
    private AudioClip targetClip;
    private FirstPersonController firstPersonController;
    private Vector3 playerVelocity;

    private void Awake()
    {
        Rojo.enabled = false;
        currentHealth = maxHealth;
        cambioEscena = GetComponent<CambioEscena>();
        firstPersonController = gameObject.GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        NotifyHealthChange();
        audioSource.loop = true;
    }

    private IEnumerator ActivarConDelay(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        SetPanelOpacity(0f);
        Rojo.enabled = false;
    }


    private void SetPanelOpacity(float alpha)
    {
        if (Rojo != null)
        {
            // Asegúrate de que el alpha esté entre 0 y 1
            alpha = Mathf.Clamp01(alpha);

            // Obtén el color actual del panel
            Color currentColor = Rojo.color;

            // Ajusta el canal alpha
            currentColor.a = alpha;

            // Asigna el nuevo color al panel
            Rojo.color = currentColor;
        }
        StartCoroutine(ActivarConDelay(1f));
    }


    // Llamar cuando recibe daño
    public void TakeDamage()
    {
        if (isDead) return;
        audioSource4.clip = DamageSound;
        audioSource4.Play();
        currentHealth -= amountDamage;
        if (currentHealth < 0)
            currentHealth = 0;
        if(currentHealth > 0)
        { 
            SetPanelOpacity(0.4f);
            Rojo.enabled = true;
            // Avisar a GameCore o a la UI
            OnPlayerDamaged?.Invoke(currentHealth);//NOTIFICAR  A GAMECORE PARA POSICIONAR NUEVAMENTE O PPERDER PARTIDA
            miPlayerHUD.UpdateHealthUI(currentHealth, maxHealth);// Para el Canvas Inventario
        }
        else
        {
            Die();
        }
            
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
        // audio CROUCH
        if (firstPersonController.crouchAudio )
        {
            audioSource4.clip = CrouchAudio;
            audioSource4.Play();
            firstPersonController.crouchAudio = false;           
        }
        // ver velocidad en Y:       
        verticalSpeed =  (transform.position.y - lastY) / Time.deltaTime;
        lastY = transform.position.y;
        //
        if (!firstPersonController.wasGrounded && firstPersonController.isGrounded && verticalSpeed < -3f)
        {// SONIDO SALTO
            audioSource4.clip = Aterrizaje;
            audioSource4.Play();
        }
        firstPersonController.wasGrounded = firstPersonController.isGrounded;

        if (!firstPersonController.lastSprintCooldown && firstPersonController.isSprintCooldown)
        { // SONIDO CANSANCIO
            audioSource4.clip = audioAgotado;
            audioSource4.Play();
        }
        firstPersonController.lastSprintCooldown = firstPersonController.isSprintCooldown;

        if (firstPersonController.isWalking || firstPersonController.isSprinting)
        {
            AudioClip targetClip = null;
            if (firstPersonController.isWalking)
            {
                targetClip = PasosSound;
            }
            if(firstPersonController.isSprinting)
            {
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
