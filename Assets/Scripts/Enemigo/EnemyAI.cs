using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    public Transform[] patrolPointsReinicio;// puntos para reiniciar la patrulla en cada nivel
    //public float detectionRadius = 8f;
    public float rotationSpeed = 8f;
    public float updateRate = 0.3f; // segundos entre actualizaciones de destino
    public NavMeshAgent agent;
    private int currentPatrolIndex;
    public bool chasingPlayerAI;// significa persiguiendo al jugador

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desactivar rotación automática
    }

    void Start()
    {
        if (patrolPoints.Length > 0)
        {   // si queremos que empiece por una posición aleatoria:
            //currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            currentPatrolIndex = 0;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        // Llamar periódicamente al sistema de decisión (en lugar de hacerlo cada frame)
        StartCoroutine(UpdateAI());
    }


    public void actPosicPatrulla(int numPtsEliminar)
    {        
        currentPatrolIndex += numPtsEliminar;
       // currentPatrolIndex %= patrolPoints.Length;
        //Debug.Log("currentPatrolIndex: " + currentPatrolIndex);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;      // Limpia la inercia
        agent.ResetPath();                  // Limpia ruta vieja
        agent.Warp(agent.transform.position); // Reancla al NavMesh
        StartCoroutine(SetDestinoSeguro());
    }
    private IEnumerator SetDestinoSeguro()
    {
        // Evitar conflicto con física
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        agent.isStopped = false;
    }


    IEnumerator UpdateAI()
    {      
        while (true)
        {            
            if (chasingPlayerAI)
            {
                // El jugador escapó, volver a patrullar
                chasingPlayerAI = false;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }

            // Si está cerca del punto de patrulla, pasar al siguiente
            if (!agent.pathPending && agent.remainingDistance < 0.6f)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;// lo ultimo es para el bucle
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
            yield return new WaitForSeconds(updateRate);
        }
    }

    public void Actualizar()
    {
        // Rotación manual suave según dirección real de movimiento
        Vector3 moveDir = agent.desiredVelocity;
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }
    }

    public void ReiniciarNivel()
    {
        string nivel  = GameCore.Instance.obtenerNivel("enemigo");
        int puntoReinicio = 0;
        if(nivel == "1")
        {
            puntoReinicio = 0;
        } else if(nivel == "2")
        {
            puntoReinicio = 1;
        }
         else if(nivel == "3")
        {
            puntoReinicio = 2;
        }
        Transform objCompartido = patrolPointsReinicio[puntoReinicio];
        agent.SetDestination(objCompartido.position);        
        currentPatrolIndex = System.Array.IndexOf(patrolPoints, objCompartido);
        Debug.Log("REINICIO NIVEL: " + nivel);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = chasingPlayerAI ? Color.red : Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}