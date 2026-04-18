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
    // private ColorGrading colorGrading;
    public Volume volumen;
    public GameObject PanelOpciones;  
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // Obtener ColorGrading del volumen
        if (volumen != null)
            volumen.profile.TryGet<ColorAdjustments>(out colorAdjustments);
     
        // Configurar slider (por ejemplo, entre -100 y +100)
        if (sliderContraste != null)
        {
            sliderContraste.minValue = -100f;
            sliderContraste.maxValue = 100f;
            sliderContraste.value = GameCore.Instance.valorContraste;           
           
            // sliderContraste.value = colorAdjustments.contrast.value;
            // Suscribirse al cambio de valor
            sliderContraste.onValueChanged.AddListener(CambiarContraste);
            sliderContraste.onValueChanged.Invoke(GameCore.Instance.valorContraste);
        }
        if (sliderSaturacion != null)
        {
            sliderSaturacion.minValue = -100f;
            sliderSaturacion.maxValue = 100f;
            sliderSaturacion.value = GameCore.Instance.valorSaturacion;
            
            //sliderSaturacion.value = colorAdjustments.contrast.value;
            // Suscribirse al cambio de valor
            sliderSaturacion.onValueChanged.AddListener(CambiarSaturacion);
            sliderSaturacion.onValueChanged.Invoke(GameCore.Instance.valorSaturacion);
        }
    }

    public void TogglePanelOpciones()
    {
        if(PanelOpciones.active == false)
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

    public void CambiarContraste(float valor)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = valor;
            GameCore.Instance.valorContraste = valor;
        }
    }
}
