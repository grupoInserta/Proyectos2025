using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public string requiredKey = "LlaveFinal";
    public Transform openPosition;
    public float openSpeed = 2f;
    private bool isOpening = false;
    public float openAngle = 90f;
    private float currentAngle = 0f;

    private void OnTriggerEnter(Collider other)
    {

        var inventory = other.GetComponent<KeyInventory>();
        if (inventory && inventory.HasKey(requiredKey))
        {
            isOpening = true;
        }
        else
        {
            Debug.Log("Necesitas la LLAVE: " + requiredKey);
        }
    }

    private void Update()
    {
        if (isOpening && currentAngle < openAngle)
        {
            float delta = openSpeed * Time.deltaTime;
            transform.Rotate(0, delta, 0);
            currentAngle += delta;
        }        
    }

}
