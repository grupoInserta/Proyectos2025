using UnityEngine;

public class Martillo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<KeyInventory>();
        if (inventory)
        {
            inventory.AddKey("Martillo");
            Destroy(gameObject); // Desaparece al recogerla
        }
    }
   
}
