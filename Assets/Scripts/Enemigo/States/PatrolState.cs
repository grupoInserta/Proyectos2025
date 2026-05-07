using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyState
{
    private int currentPointIndex;
    private float waitTime = 2f;
    private float waitTimer;    
    //public float detectionRadius = 8f; 
    public float updateRate = 0.3f; // segundos entre actualizaciones de destino
    public int currentPatrolIndex;
    public bool pierdoVistaJugador;
    private Coroutine patrolRoutine;

    public PatrolState(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    
    public override void Enter()
    {
        if (enemy.patrolPoints.Length > 0)
        {   // si queremos que empiece por una posición aleatoria:
            //currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            currentPatrolIndex = 0;
            enemy.agent.SetDestination(enemy.patrolPoints[currentPatrolIndex].position);
        }
        enemy.ChasingPlayerAI = false;
        enemy.pierdoVistaJugador = false;
        patrolRoutine = enemy.RunCoroutine(UpdateAI());
        /****enemyAnimationController.PlayAttack();***/
    }

    IEnumerator UpdateAI() // es repetitivo pero con unos segundos de intervalo personalizables al final del metodo
    {       
        while (true)
        {
            if (pierdoVistaJugador)
            {
                // El jugador escapó, volver a patrullar
                pierdoVistaJugador = false;
                enemy.ChasingPlayerAI = false;
                enemy.agent.SetDestination(enemy.patrolPoints[currentPatrolIndex].position);
            }

            // Si está cerca del punto de patrulla, pasar al siguiente
            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.6f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % enemy.patrolPoints.Length;// lo ultimo es para el bucle
                enemy.agent.SetDestination(enemy.patrolPoints[currentPatrolIndex].position);
            }
            yield return new WaitForSeconds(updateRate);
        }
    }

    public void actPosicPatrulla(int numPtsEliminar)
    {       
        currentPatrolIndex += numPtsEliminar;
        // currentPatrolIndex %= patrolPoints.Length;
        //Debug.Log("currentPatrolIndex: " + currentPatrolIndex);
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;      // Limpia la inercia
        enemy.agent.ResetPath();                  // Limpia ruta vieja
        enemy.agent.Warp(enemy.agent.transform.position); // Reancla al NavMesh
        patrolRoutine = enemy.RunCoroutine(SetDestinoSeguro());
    }

    private IEnumerator SetDestinoSeguro()
    {
        // Evitar conflicto con física
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //POSIBLE ERROR EN LINEA INF
        enemy.agent.SetDestination(enemy.patrolPoints[currentPatrolIndex].position);
        enemy.agent.isStopped = false;
    }

    

    public void ReiniciarNivel(int numPtosDentro)
    {
        //**************************************************************
        /* hago que si se trata del nivel 2 y son las puertas de estrada (porque miro el numero de puntos internos)
        con lo que reinicio arriba
         */

        string nivel = GameCore.Instance.obtenerNivel("enemigo");
        int puntoReinicio = 0;
        if (nivel == "1")
        {
            puntoReinicio = 0;
        }
        else if (nivel == "2")
        {
            if (numPtosDentro == 17 || numPtosDentro == 19)
            {
                puntoReinicio = 0;
            }
            else
            {
                puntoReinicio = 1;
            }
        }
        else if (nivel == "3")
        {
            puntoReinicio = 0;
            //puntoReinicio = 2; que se reinicie en el mismo nivel o en otros dependiendo dep azar u otro criterio
        }
        Transform objCompartido = enemy.patrolPointsReinicio[puntoReinicio];
        enemy.agent.SetDestination(objCompartido.position);
        currentPatrolIndex = System.Array.IndexOf(enemy.patrolPoints, objCompartido);

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = enemy.ChasingPlayerAI ? Color.red : Color.yellow;
    }

}