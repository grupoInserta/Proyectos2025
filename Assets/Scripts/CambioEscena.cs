using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip ClicSound;
    public void CargaEscena(string nombreEscenaCarga)
    {
        if(audioSource != null)
        {
            audioSource.PlayOneShot(ClicSound);           
        }
        
        SceneManager.LoadScene(nombreEscenaCarga);
    }
    
}
