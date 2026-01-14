using UnityEngine;
using System.Collections;
public class Door : MonoBehaviour
{
    public string requiredKey;
    public float openSpeed = 2f;
    private bool isOpening = false;
    private bool tieneLaLlave = false;
    private bool playerCercano = false;
    private bool puertaAbierta = false;
    // apertura puerta por rotacion:
    public float openAngle;//slot
    private float currentAngle = 0f;
    private FirstPersonController PlayerScript;
    private KeyInventory inventory;
    private bool yaDesactivado = false;
    public bool TOC { get; set; }
    public int numPosicionesDentro;
    private BoxCollider[] boxes;
    public Collider myCollider;


    private void Awake()
    {
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCercano = true;
            if(tieneLaLlave && PlayerScript.pulsadoAbrir)
            {
                isOpening = true;
               // PlayerScript.pulsadoAbrir = false;                
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {   if (puertaAbierta) return;   
        if (other.CompareTag("Player"))
        {
            PlayerScript = other.GetComponent<FirstPersonController>();
            inventory = other.GetComponent<KeyInventory>();           

            if (inventory && inventory.HasKey(requiredKey))
            {
                tieneLaLlave = true;
                // por ahora no quitamos la llave
                // inventory.RemoveKey(requiredKey);
                if (!puertaAbierta && currentAngle < 0.1f)                  
                {
                    inventory.MostrarAviso("Pulsa E para abrir");
                }
                              
            }            
        }
    }

    private IEnumerator DesactivarTrasRetraso(float delay)
    {        
        yield return new WaitForSeconds(delay);
        boxes = GetComponents<BoxCollider>();
        foreach (BoxCollider box in boxes)
        {
            box.enabled = false;
        }
        this.enabled = false;  // desactiva el script después de terminar de abrir
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCercano = false;
            PlayerScript.pulsadoAbrir = false;
            inventory.MostrarAviso("");
        }
    }

    private void Update()
    {        
        if (isOpening && currentAngle < openAngle )
        {
            inventory.MostrarAviso("");
            float delta = openSpeed * Time.deltaTime;
            transform.Rotate(0, delta, 0);
            currentAngle += delta;

        } else if (isOpening && currentAngle >= openAngle)
        {
            Debug.Log("DESACTIVO");
            isOpening = false;
            puertaAbierta = true;
            myCollider = gameObject.GetComponent<Collider>();
            myCollider.enabled = false;
            if (!yaDesactivado)
            {// DESACTIVAMOS AQUI PARA MEJOR RENDIMIENTO DE LA APLICACIÓN
                StartCoroutine(DesactivarTrasRetraso(1.0f));
                yaDesactivado = true;
            }            
        }
        if (PlayerScript == null) return;
        if (!tieneLaLlave && PlayerScript.pulsadoAbrir && playerCercano)
        {
            inventory.MostrarAviso("Necesitas la llave: " + requiredKey);           
            PlayerScript.pulsadoAbrir = false;
        }
    }
}
