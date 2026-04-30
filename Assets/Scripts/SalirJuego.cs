using UnityEngine;

public class SalirJuego : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip ClicSound;
    public void SalirDelJuego()
    {
        audioSource.PlayOneShot(ClicSound);
        Application.Quit(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
