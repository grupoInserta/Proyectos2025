using UnityEngine;

public class Radio : MonoBehaviour
{

    public AudioSource audioSource5;
    public AudioClip GrabacionNivel1;
    private bool reproduciendo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reproduciendo = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && reproduciendo == false)
        {
            reproduciendo = true;
            Debug.Log("Reproducir sonido");
            audioSource5.clip = GrabacionNivel1;
            audioSource5.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            reproduciendo = false;
            audioSource5.Stop();
        }
    }

     
}
