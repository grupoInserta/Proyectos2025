using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class AjustesPantalla : MonoBehaviour
{
   // public PostProcessVolume volume;
    public Slider sliderContraste;
    public Slider sliderSaturacion;
    public Slider sliderBrillo;
    public Slider sliderVolumenSonido;
    // private ColorGrading colorGrading;
    public Volume volumen;
    public GameObject PanelOpciones;  
    private ColorAdjustments colorAdjustments;
    //
    public AudioClip ClicSound;
    public AudioSource audioSource;
    // ajustes volumen sonido
 

    void Start()
    {
        // Obtener ColorGrading del volumen
        if (volumen != null)
            volumen.profile.TryGet<ColorAdjustments>(out colorAdjustments);
     
        // Configurar slider (por ejemplo, entre -80 y +80)
        if (sliderContraste != null)
        {
            sliderContraste.minValue = -80f;
            sliderContraste.maxValue = 80f;
            sliderContraste.value = GameCore.Instance.valorContraste;           
           
            // sliderContraste.value = colorAdjustments.contrast.value;
            // Suscribirse al cambio de valor
            sliderContraste.onValueChanged.AddListener(CambiarContraste);
            sliderContraste.onValueChanged.Invoke(GameCore.Instance.valorContraste);
        }
        if (sliderSaturacion != null)
        {
            sliderSaturacion.minValue = -80f;
            sliderSaturacion.maxValue = 80f;
            sliderSaturacion.value = GameCore.Instance.valorSaturacion;
            
            //sliderSaturacion.value = colorAdjustments.contrast.value;
            // Suscribirse al cambio de valor
            sliderSaturacion.onValueChanged.AddListener(CambiarSaturacion);
            sliderSaturacion.onValueChanged.Invoke(GameCore.Instance.valorSaturacion);
        }
        if (sliderBrillo != null)
        {
            sliderBrillo.minValue = -2f;
            sliderBrillo.maxValue = 2f;
            sliderBrillo.value = GameCore.Instance.valorBrillo;

            //sliderSaturacion.value = colorAdjustments.contrast.value;
            // Suscribirse al cambio de valor
            sliderBrillo.onValueChanged.AddListener(CambiarBrillo);
            sliderBrillo.onValueChanged.Invoke(GameCore.Instance.valorBrillo);
        }
        // SONIDO, VOLUMEN
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sliderVolumenSonido.value = volume;
        AudioListener.volume = volume;
        // escuchar cambios
        sliderVolumenSonido.onValueChanged.AddListener(ChangeVolume);
    }

    private void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void TogglePanelOpciones()
    {
        audioSource.PlayOneShot(ClicSound);
        if (PanelOpciones.active == false)
        {            
            PanelOpciones.active = true;
        }
        else
        {
            PanelOpciones.active = false;
        }        
    }
    

    public void CambiarSaturacion(float valor)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = valor;
            GameCore.Instance.valorSaturacion = valor;
        }
    }

    public void CambiarBrillo(float valor)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = valor;
            GameCore.Instance.valorBrillo = valor;
        }
    }

    public void CambiarContraste(float valor)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = valor;
            GameCore.Instance.valorContraste = valor;
        }
    }
}
