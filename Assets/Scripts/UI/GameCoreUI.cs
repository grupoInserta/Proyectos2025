using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class GameCoreUI : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasGameManager;
    [SerializeField]
    private Button botonPausa;
    [SerializeField]
    private Button botonInicio;
    [SerializeField]
    private Canvas PanelInicio;// panel que se ve por defecto al iniciar un juego y da a elegir entre jugar nuevo o anterior

    [SerializeField]
    private Canvas PanelCanvasGamesCore;
    [SerializeField]
    private GameObject CanvasAjustesManager;
    [SerializeField]
    private Canvas HUDPlayer;

    public bool JuegoPausado { get; set; }
    private InputAction pause;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private FirstPersonController PlayerScript;
    private AudioSource audioSource;
    public AudioClip ClicSound;
    public AudioClip InicioPlayerSound;
    private bool JuegoIniciado = false;

    void OnEnable()
    {
        pararJuego();
        pause = new InputAction("Pause", InputActionType.Button);
        pause.AddBinding("<Gamepad>/start");
        pause.AddBinding("<Keyboard>/escape");
        pause.Enable();
        HUDPlayer.enabled = false;
        pause.performed += ctx => MostrarPanel();
        //
        botonPausa.onClick.AddListener(() => PausarReanudar());
        botonInicio.onClick.AddListener(() => IrAInicio());
        //
        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    private IEnumerator SonidoPlayerConDelay(float segundos)
    {
        yield return null; // esperar 1 frame
        audioSource.clip = InicioPlayerSound;
        audioSource.Play();
    }

    public void CerrarPanelInicio()
    {
        Reanudar();       
        JuegoPausado = false;
        PanelInicio.enabled = false;
        HUDPlayer.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioSource.PlayOneShot(ClicSound);
        if (!JuegoIniciado) {
            StartCoroutine(SonidoPlayerConDelay(1f));
            JuegoIniciado = true;
        }        
    }

    private void MostrarPanel()
    {
        if (PanelInicio.enabled == true) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CanvasGameManager.SetActive(true);
        pararJuego();
        PlayerScript.DesactivarCrossHair();   
        audioSource.PlayOneShot(ClicSound);
    }
    
    public void TogglePanelAjustes()
    {
        if(CanvasAjustesManager.activeSelf == false)
        {
            MostrarPanelAjustes();
        }
        else
        {
            CerrarPanelAjustes();
        }
        
    }
    private void MostrarPanelAjustes()
    {
       // if (PanelCanvasGamesCore.enabled == true) return;
        CanvasAjustesManager.SetActive(true);        
        audioSource.PlayOneShot(ClicSound);
    }

    private void CerrarPanelAjustes()
    {
        PanelCanvasGamesCore.enabled = true;
        CanvasAjustesManager.SetActive(false);
        audioSource.PlayOneShot(ClicSound);
    }

    void IrAInicio()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
        audioSource.PlayOneShot(ClicSound);
    }

    void OnDisable()
    {
        pause.Disable();
    }

    private void Reanudar()
    {
        CerrarPanelAjustes();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        JuegoPausado = false;
        Time.timeScale = 1f;  // Reanuda el juego
        CanvasGameManager.SetActive(false);        
        PlayerScript.enabled = true;
        PlayerScript.ActivarCrossHair();
        audioSource.PlayOneShot(ClicSound);
        GameCore.Instance.TogglePause();
    }

    private void pararJuego()
    {
        Time.timeScale = 0f;// Pausa todo el juego
        PlayerScript.enabled = false;
        JuegoPausado = true;
        GameCore.Instance.TogglePause();
    }

    private void PausarReanudar()
    {
        if (!JuegoPausado)
        {
            pararJuego();            
        }
        else
        {           
            Reanudar();
        }
        audioSource.PlayOneShot(ClicSound);       
    }

}
