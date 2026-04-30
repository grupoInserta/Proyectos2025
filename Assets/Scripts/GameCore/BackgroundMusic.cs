using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip musicClip;
    public AudioClip DerrotaSound;
    public AudioClip VictoriaSound;
    public AudioClip NivelSound;
    public AudioClip MenuPrincipalSound;
    [Range(0f, 1f)] public float volume = 0.5f;




   public void Reproducir(string nombreEscena)
    {
        if(nombreEscena == "Victoria")
        {
            musicClip = VictoriaSound;
        }
        else if(nombreEscena == "Derrota")
        {
            musicClip = DerrotaSound;
        }
        else if (nombreEscena == "Nivel")
        {
            musicClip = NivelSound;
        }
        else if (nombreEscena == "MenuPrincipal")
        {
            musicClip = MenuPrincipalSound;
        }

        if (audioSource == null)
       
        audioSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }
}