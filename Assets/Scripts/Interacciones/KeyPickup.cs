using UnityEngine;
// ESTE SCRIPT VA COMO COMPONENTE EN LAS LLAVES
public class KeyPickup : MonoBehaviour
{
    public string keyName; // ej: "verde"
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<KeyInventory>();
        if (inventory)
        {
            inventory.AddKey(keyName);
            Destroy(gameObject); // Desaparece al recogerla
        }
    }
}
