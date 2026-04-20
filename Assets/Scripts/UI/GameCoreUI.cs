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
    public bool JuegoPausado { get; set; }
    private InputAction pause;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private FirstPersonController PlayerScript;

    void OnEnable()
    {
        pararJuego();
        pause = new InputAction("Pause", InputActionType.Button);
        pause.AddBinding("<Gamepad>/start");
        pause.AddBinding("<Keyboard>/escape");
        pause.Enable();
        pause.performed += ctx => MostrarPanel();
        //
        botonPausa.onClick.AddListener(() => PausarReanudar());
        botonInicio.onClick.AddListener(() => IrAInicio());
    }

    public void CerrarPanelInicio()
    {
        Reanudar();
        JuegoPausado = false;
        PanelInicio.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MostrarPanel()
    {
        if (PanelInicio.enabled == true) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CanvasGameManager.SetActive(true);
        pararJuego();
    }
    

    void IrAInicio()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
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
                              // botonPausa.GetComponentInChildren<TextMeshProUGUI>().text = "Pausar";
        CanvasGameManager.SetActive(false);
        PlayerScript.enabled = true;
    }

    private void pararJuego()
    {
        Time.timeScale = 0f;// Pausa todo el juego
        PlayerScript.enabled = false;
        JuegoPausado = true;
        //botonPausa.GetComponentInChildren<TextMeshProUGUI>().text = "Continuar";
    }

    private void PausarReanudar()
    {
        if (!JuegoPausado)
        {
            pararJuego();            
        }
        else
        {
            Debug.Log("reanudar");
           // botonPausa.GetComponentInChildren<TextMeshProUGUI>().text = "Parar";
            Reanudar();
        }
    }

}
