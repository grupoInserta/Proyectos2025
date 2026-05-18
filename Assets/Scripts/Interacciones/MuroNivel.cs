using UnityEngine;

public class MuroNivel : MonoBehaviour
{
    private CambioEscena cambioEscena;
    [SerializeField] private bool activarDemo;
    [SerializeField] private bool activoMuroFinal;
    public AudioSource audioSource;
    public AudioClip PasoNivel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cambioEscena = GetComponent<CambioEscena>();
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
                audioSource.PlayOneShot(PasoNivel);
            }
        }
    }
}
