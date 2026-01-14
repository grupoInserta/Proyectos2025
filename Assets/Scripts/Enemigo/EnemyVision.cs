using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    [Header("Componentes")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Parámetros de visión")]
    public float visionRadius = 15f;// distancia máxima de visión
    private float RadiusminPersec = 5f;//cuando esta persiguiendo y pierde de vista estando cerca
    public float visionAngle = 90f;          // campo de visión en grados
    public LayerMask obstacleMask;           // capas que bloquean la vista
    public LayerMask playerMask;             // capa del jugador

    //private bool chasingPlayer = false;
    private bool chasingPlayerAI; // lo persiguen viendolo
    private float distanceToPlayer;
    //
    private float tiempoMemoria = 3f;
    private float tiempoUltimaVista;
    private EnemyAI miEnemyAI;
    private float velocidadRotacion = 0.6f;
    private bool ActualizandoEnemyAI = true;
    private bool mirandoPlayer = false;
  

    private void Start()
    {
        player = gameObject.GetComponent<EnemyAI>().player;
        agent = gameObject.GetComponent<EnemyAI>().agent;
        miEnemyAI = gameObject.GetComponent<EnemyAI>();
        chasingPlayerAI = miEnemyAI.chasingPlayerAI;
    }

    void Update()
    {
        if (PuedeVerAlJugador())
        {
            Debug.Log("PERSIGUIENDO AL JUGADOR y ActualizandoEnemyAI: "+ ActualizandoEnemyAI);
            // chasingPlayer = true;
            chasingPlayerAI = true;
            agent.SetDestination(player.position);
            tiempoUltimaVista = Time.time;
        }
        else if ((Time.time - tiempoUltimaVista < tiempoMemoria) && (distanceToPlayer < RadiusminPersec))
        {   //
            // Sigue buscando en la última posición vista
            agent.SetDestination(player.position);
            //tiempoUltimaVista = 0;?
        }
        else
        {
            chasingPlayerAI = false;
        }
        if(ActualizandoEnemyAI)
            miEnemyAI.Actualizar();
    }

    private void OnTriggerEnter(Collider other)
    {       
        if (other.CompareTag("Puerta") )
        {            
            Door puerta = other.GetComponent<Door>();           
            if (puerta == null) return;
            if (puerta.TOC == false)
            {
                puerta.TOC = true;
                Debug.Log("TOC, nivel: " + GameCore.Instance.obtenerNivel("enemigo"));
                int numPosicionesDentro = puerta.numPosicionesDentro;
                // Evitar múltiples activaciones
                // other.enabled = false;// si lo dejo, la puerta tiene cillider desactivado
                // y no sirve tampoco para evitar una segunda colisión..
                StartCoroutine(ActualizarPatrullaConDelay(numPosicionesDentro));
            }
            else if (puerta.TOC == true) // ha completado una patrulla en el nivel ya y ha intentado pasar por la puerta..
            {
                //*****************************
                miEnemyAI.ReiniciarNivel();
                puerta.TOC = false;
                // implementar que vuelva al punto de inicio del nivel...
                // hay que designar en cada nivel un punto de reinicio si no puede pasar de nivel por la escalera...
            }
        }        
        else if(other.CompareTag("Player")){
            PlayerHealth saludJugador = other.GetComponent<PlayerHealth>();
            saludJugador.TakeDamage(20);
        }
    }

    private IEnumerator ActualizarPatrullaConDelay(int numPtsEliminar)
    {
        yield return null; // esperar 1 frame
        miEnemyAI.actPosicPatrulla(numPtsEliminar);
    }


    bool PuedeVerAlJugador()
    {
        // 1️ Distancia
        Vector3 dirToPlayer = player.position - transform.position;        
        distanceToPlayer = dirToPlayer.magnitude;
        if (distanceToPlayer > visionRadius)
            return false;   

        // 2️ Ángulo      
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer.normalized);

        if (angleToPlayer > visionAngle / 2f)
        {
            return false;
        } else {
            ActualizandoEnemyAI = false; //Esto hace que intente mirar hacia el player..
            //.. y para ello paramos patrulla con ActualizandoEnemyAI = false
            dirToPlayer.y = 0; // Evita inclinaciones en el eje vertical
            Debug.Log("INTENTANDO MIRAR AL JUGADOR");
            if (dirToPlayer != Vector3.zero)
            { // MIRAR
                Quaternion rotacionObjetivo = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rotacionObjetivo,
                    velocidadRotacion * Time.deltaTime
                );
                // cuando ya mira en direccion al player aunque este detrás de un obstaculo:
                float diferencia = Quaternion.Angle(transform.rotation, rotacionObjetivo);
                if (diferencia < 1f)
                {
                    //una mirada hacia el jugador..
                    mirandoPlayer = true;
                }
                else
                {
                    mirandoPlayer = false;
                }
            }
        }  // angulo zona vision         

        // 3️ Línea de visión (raycast)
        if (mirandoPlayer && Physics.Raycast(transform.position + Vector3.up * 0.5f, dirToPlayer.normalized, out RaycastHit hit, visionRadius, obstacleMask | playerMask))
        {
            Debug.Log("NOMBRE: " + hit.collider.gameObject.name);
            if (hit.transform == player)
            {
                return true; //  jugador visible
            }
            else
            {
                Debug.Log("vuelvo a mi ruta");
                ActualizandoEnemyAI = true; // hacemos que justo despues de lanzar un rayo, no lo ve y sige su patrulla
            }
        }
        return false; // X obstáculo o fuera de vista
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el campo de visión en el editor
      
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 leftLimit = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftLimit * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit * visionRadius);
    }
}
