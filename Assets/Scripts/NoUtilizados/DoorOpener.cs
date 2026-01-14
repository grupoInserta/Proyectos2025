using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Configuración de la puerta")]
    public Transform doorPivot;     // Punto sobre el que gira la puerta
    public float openAngle = 90f;   // Ángulo de apertura en grados
    public float openSpeed = 2f;    // Velocidad de apertura
    public bool autoClose = true;   // Si se cierra sola al salir
    public float closeDelay = 2f;   // Tiempo antes de cerrarse

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private float closeTimer = 0f;

    void Start()
    {
        // Guardamos la rotación inicial (cerrada)
        closedRotation = doorPivot.localRotation;
        // Calculamos la rotación de la puerta abierta
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        // Interpolamos suavemente la rotación hacia el estado actual
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorPivot.localRotation = Quaternion.Slerp(
            doorPivot.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );

        // Si debe cerrarse automáticamente
        if (autoClose && isOpen)
        {
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
                isOpen = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;
            closeTimer = closeDelay;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            closeTimer = closeDelay; // Mantiene abierta mientras el jugador esté dentro
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && autoClose)
        {
            closeTimer = closeDelay; // empieza a contar para cerrar
        }
    }
}
