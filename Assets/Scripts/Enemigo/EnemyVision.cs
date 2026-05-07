using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyVision : MonoBehaviour
{
    [Header("Componentes")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Parámetros de visión")]
    public float visionRadius;// distancia máxima de visión
    private float RadiusminPersec = 15f;//cuando esta persiguiendo y pierde de vista estando cerca
    public float visionAngle = 90f;          // campo de visión en grados
    public LayerMask obstacleMask;           // capas que bloquean la vista
    public LayerMask playerMask;             // capa del jugador

    //private bool chasingPlayer = false;
    //private bool chasingPlayerAI; // lo persiguen viendolo
    private float distanceToPlayer;
    private float tiempoMemoria = 3f;
    private float tiempoUltimaVista;
    private EnemyAI miEnemyAI;
    private float velocidadRotacion = 0.6f;
    private bool ActualizandoEnemyAI = true;
    private bool mirandoPlayer = false;
    // SONIDOS
    public AudioClip ataqueSound;
    public AudioClip sonidoAvistado;
    public AudioSource audioSource;
    private bool sonidoPersecucion = false;
    public float distanciaAtaque = 4.5f;

    // quitar
    int contador = 0;
    private void Start()
    {
        player = gameObject.GetComponent<EnemyAI>().player;
        agent = gameObject.GetComponent<EnemyAI>().agent;
        miEnemyAI = gameObject.GetComponent<EnemyAI>();
        audioSource.clip = sonidoAvistado;
        audioSource.loop = true;
       
    }

    private void ReproducirSonidoAtaque()
    {      
        audioSource.PlayOneShot(ataqueSound);        
    }

    void Update()
    {       
        if (PuedeVerAlJugador())
        {
            if (!sonidoPersecucion)
            {
                audioSource.clip = sonidoAvistado;
                audioSource.loop = true;
                audioSource.Play();
                sonidoPersecucion = true;
            }
            
            Debug.Log("PERSIGUIENDO AL JUGADOR y ActualizandoEnemyAI: "+ ActualizandoEnemyAI);
            // chasingPlayer = true;
            miEnemyAI.ChasingPlayerAI = true;
            agent.SetDestination(player.position);
            tiempoUltimaVista = Time.time;
            //
            Vector3 dirToPlayer = player.position - transform.position;
            distanceToPlayer = dirToPlayer.magnitude;
            Debug.Log("distanceToPlayer: " + distanceToPlayer + "distanciaAtaque: " + distanciaAtaque);
            //
            if (distanceToPlayer < distanciaAtaque && miEnemyAI.IsAttackingAI == false)
            {
                ReproducirSonidoAtaque();
                miEnemyAI.IsAttackingAI = true;
            }
        }
        else if ((Time.time - tiempoUltimaVista < tiempoMemoria) && (distanceToPlayer < RadiusminPersec))
        {   //
            // Sigue buscando en la última posición vista
            agent.SetDestination(player.position);
            sonidoPersecucion = false;
            //tiempoUltimaVista = 0;?
        }       
        else
        {
            // PIERDE DE VISTA AL JUGADOR
            sonidoPersecucion = false;
            miEnemyAI.pierdoVistaJugador = true;
            audioSource.Pause();
            // miEnemyAI.chasingPlayerAI = false;
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
            int numPosicionesDentro = puerta.numPosicionesDentro;
            if (puerta.TOC == false) 
            {
                contador++;
                puerta.TOC = true;
                Debug.Log("TOC, nivel: " + GameCore.Instance.obtenerNivel("enemigo"));
                string nivel = GameCore.Instance.obtenerNivel("enemigo");
                if (numPosicionesDentro == 1000 && nivel == "1")
                {
                    miEnemyAI.ReiniciarNivel(numPosicionesDentro);
                    StartCoroutine(ActualizarPatrullaConDelay(0));
                }
                else
                {
                    // Evitar múltiples activaciones
                    // other.enabled = false;// si lo dejo, la puerta tiene collider desactivado
                    // y no sirve tampoco para evitar una segunda colisión..
                    StartCoroutine(ActualizarPatrullaConDelay(numPosicionesDentro));
                }               
            }
            else if (puerta.TOC == true) // ha completado una patrulla en el nivel ya y ha intentado pasar por la puerta..
            {
                miEnemyAI.ReiniciarNivel(numPosicionesDentro);
                puerta.TOC = false;
                // implementar que vuelva al punto de inicio del nivel...
                // hay que designar en cada nivel un punto de reinicio si no puede pasar de nivel por la escalera...
            }
        }        
        else if(other.CompareTag("Player")){
            PlayerHealth saludJugador = other.GetComponent<PlayerHealth>();
            saludJugador.TakeDamage(20); //
            miEnemyAI.IsAttackingAI = false;
            audioSource.Pause();
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
            //ActualizandoEnemyAI = false; //Esto hace que intente mirar hacia el player..
            //.. y para ello paramos patrulla con ActualizandoEnemyAI = false
            dirToPlayer.y = 0; // Evita inclinaciones en el eje vertical
           
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
                
                if (diferencia < 40f)
                {
                    //una mirada hacia el jugador..
                    mirandoPlayer = true;
                    Debug.Log("LO MIRO BIEN ignorando obstaculos");
                }
                else
                {
                    mirandoPlayer = false;
                    Debug.Log("LO MIRO PERO NO PUEDO verlo");
                }
            }
        }  // angulo zona vision         

        // 3️ Línea de visión (raycast)
        int mask = obstacleMask | playerMask;
        mask &= ~LayerMask.GetMask("Transparente"); // excluir capa

        if (mirandoPlayer && Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, visionRadius, mask, QueryTriggerInteraction.Ignore))
        {

            if (hit.transform.root == player)
            {
                Debug.Log("LO VEOOOOO!!!");
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
        // vision     

        // Draw the line
        Gizmos.color = Color.green;

        Vector3 origin = transform.position;
        //+ Vector3.up * 1.5f; // altura ojos
        Vector3 direction = transform.forward * 50f;

        Gizmos.DrawRay(origin, direction);
    }
}
