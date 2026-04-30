using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip ClicSound;
    public void CargaEscena(string nombreEscenaCarga)
    {
        audioSource.PlayOneShot(ClicSound);
        SceneManager.LoadScene(nombreEscenaCarga);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
