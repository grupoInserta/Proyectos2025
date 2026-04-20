using UnityEngine;

public class MuroDemo : MonoBehaviour
{
    private CambioEscena cambioEscena;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cambioEscena = GetComponent<CambioEscena>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cambioEscena.CargaEscena("Victoria");
        }
    }
}
