using UnityEngine;

public class MuroDemo : MonoBehaviour
{
    private CambioEscena cambioEscena;
    [SerializeField] private bool activarDemo;
    public AudioSource audioSource;
    public AudioClip PasoNivel;
    private bool puedeSonar = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cambioEscena = GetComponent<CambioEscena>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puedeSonar = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (!puedeSonar)
            return;
         if (other.CompareTag("Player"))
        {
            if (activarDemo)
            {
                cambioEscena.CargaEscena("Victoria");
            }
            else
            {
                audioSource.PlayOneShot(PasoNivel);
                puedeSonar = false;
            }      
        }
    }
}
