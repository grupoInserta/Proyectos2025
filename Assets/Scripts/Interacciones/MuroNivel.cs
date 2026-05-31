using UnityEngine;

public class MuroNivel : MonoBehaviour
{
    private CambioEscena cambioEscena;
    [SerializeField] private bool activarDemo;
    [SerializeField] private bool activoMuroFinal;
    public AudioSource audioSource;
    public AudioClip PasoNivel;
    private bool nivelPasado = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cambioEscena = GetComponent<CambioEscena>();
    }

  
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nivelPasado = false;
        }
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (activarDemo || activoMuroFinal)
            {
                cambioEscena.CargaEscena("Victoria");
            }
            else
            {
                if(audioSource != null && PasoNivel != null && nivelPasado == false)
                {
                    audioSource.PlayOneShot(PasoNivel);
                    nivelPasado = true;
                }
                   
            }
        }
    }
}
