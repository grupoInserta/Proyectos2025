using UnityEngine;

public class SonidoZonaSegura : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameCore.Instance.BGM.Reproducir("Nivel");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameCore.Instance.BGM.Reproducir("ZonaSegura");
        }
    }
}
