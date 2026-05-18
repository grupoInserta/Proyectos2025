using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;



public class PlayerHealth : MonoBehaviour
{
    
    //Acceder a las opciones de rendering del proyecto para poder acceder al componente Lens Distortion
    



    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public PlayerHUD miPlayerHUD;
    public Image Rojo;

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
    public AudioClip PreCrouchSound;
    public AudioClip DamageSound;
    private AudioClip targetClip;
    private FirstPersonController firstPersonController;

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

    private IEnumerator SonidoConDelay(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        audioSource4.clip = Aterrizaje;
        audioSource4.Play();


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
        if (firstPersonController.isGrounded == true && firstPersonController.isJumping == true)
        {
            firstPersonController.isJumping = false;
            StartCoroutine(SonidoConDelay(1.1f));
        }
        
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
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                targetClip = PreCrouchSound;
            }
            if (firstPersonController.isCrouched && firstPersonController.isWalking)
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
