using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameCoreUI : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasGameManager;
    [SerializeField]
    private Button botonPausa;
    [SerializeField]
    private Button botonInicio;
    [SerializeField]
    private Canvas PanelInicio;

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

    public void CerrarPanelInicio()
    {
        Reanudar();
        JuegoPausado = false;
        PanelInicio.enabled = false;
        HUDPlayer.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioSource.PlayOneShot(ClicSound);
    }

    private void MostrarPanel()
    {
        if (PanelInicio.enabled == true) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CanvasGameManager.SetActive(true);
        pararJuego();
        audioSource.PlayOneShot(ClicSound);
    }
    
    public void MostrarPanelAjustes()
    {
        if (PanelCanvasGamesCore.enabled == true) return;
        CanvasAjustesManager.SetActive(true);
        audioSource.PlayOneShot(ClicSound);
    }

    public void CerrarPanelAjustes()
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        JuegoPausado = false;
        Time.timeScale = 1f;  // Reanuda el juego
        CanvasGameManager.SetActive(false);
        PlayerScript.enabled = true;
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
            Reanudar(); Debug.Log("reanudar");
        }
        audioSource.PlayOneShot(ClicSound);       
    }

}
