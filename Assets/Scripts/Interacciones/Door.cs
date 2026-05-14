using UnityEngine;
using System.Collections;
public class Door : MonoBehaviour
{
    public string requiredKey; // el color en el Inventario
    public float openSpeed = 2f;
    private bool isOpening = false;
    private bool activadoAbrir = false;
    private bool tieneLaLlave = false;
    private bool playerCercano = false;
    public bool puertaAbierta = false;
    private Quaternion PosicionInicial;
    private Vector3 AlturaInicial;
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
    //
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip noMartilloSound;


    private void Awake()
    {
        PosicionInicial = transform.rotation;
        AlturaInicial = transform.position;
    }

    public void ReiniciarPuerta()
    {
        if(requiredKey == "Martillo")
        {
            transform.position = AlturaInicial;
        }
        
        if (puertaAbierta)
        {
            transform.rotation = PosicionInicial;
        }        
    }

    public void CargarPosicionPuerta()
    {   
        transform.Rotate(0,openAngle,0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript = other.GetComponent<FirstPersonController>();
            PlayerScript.contactoConPuerta = false;
            PlayerScript.pulsadoAbrir = false;
            playerCercano = false;
            inventory.MostrarAviso("");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript = other.GetComponent<FirstPersonController>();
            PlayerScript.contactoConPuerta = true;
            if (requiredKey == "Martillo" )
            {
                if (tieneLaLlave)
                {
                    transform.Translate(0f, 1000f, 0f);
                    //Destroy(gameObject);
                } else
                {
                    inventory.MostrarAviso("Necesitas el martillo!!!");
                    audioSource.PlayOneShot(noMartilloSound); 
                }                
                return; 
            } 
            playerCercano = true;
            if(tieneLaLlave && PlayerScript.pulsadoAbrir)
            {                
                isOpening = true; // esto abre la  puerta
                PlayerScript.pulsadoAbrir = false;
                if (!activadoAbrir) audioSource.PlayOneShot(openSound);
                activadoAbrir = true;// para el sonido una unica vez
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
                if (!puertaAbierta && currentAngle < 0.1f && requiredKey != "Martillo")                  
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
            isOpening = false;
            puertaAbierta = true;
            GameCore.Instance.RegisterDoorOpen(gameObject.name);
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
            audioSource.PlayOneShot(noMartilloSound);
            PlayerScript.pulsadoAbrir = false;
        }
    }
}
