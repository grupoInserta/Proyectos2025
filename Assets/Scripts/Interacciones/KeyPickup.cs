using UnityEngine;
// ESTE SCRIPT VA COMO COMPONENTE EN LAS LLAVES
public class KeyPickup : MonoBehaviour
{
    public string keyName; // ej: "verde"
    public AudioClip Seleccionada;
    public AudioSource audioSource;

    
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<KeyInventory>();
        if (inventory)
        {
            inventory.AddKey(keyName, false);
            audioSource.PlayOneShot(Seleccionada);
            Destroy(gameObject); // Desaparece al recogerla
        }
    }
}
