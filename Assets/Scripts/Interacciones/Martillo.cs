using UnityEngine;

public class Martillo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource audioSource;
    public AudioClip Seleccionado;
    
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<KeyInventory>();
        if (inventory)
        {
            audioSource.PlayOneShot(Seleccionado);
            inventory.AddKey("Martillo", false);
            Destroy(gameObject); // Desaparece al recogerla
        }
    }
   
}
