using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{


    public void CargaEscena(string nombreEscenaCarga)
    {
        SceneManager.LoadScene(nombreEscenaCarga);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
