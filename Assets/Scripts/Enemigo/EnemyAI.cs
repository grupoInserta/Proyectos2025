using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public IdleState IdleState { get; private set; }
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }
    public SearchState SearchState { get; private set; }
    //*************//
    public Transform player;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public Transform[] patrolPointsReinicio;// puntos para reiniciar la patrulla en cada nivel
    public bool ChasingPlayerAI;// significa persiguiendo al jugador
    public bool IsAttackingAI;
    public bool IsSearchingAI;
    public bool pierdoVistaJugador;
    public float rotationSpeed = 8f;
    private EnemyAnimationController enemyAnimationController;
    //*************//

    private void Awake()
    {
        StateMachine = new StateMachine();

        IdleState = new IdleState(this, StateMachine);
        PatrolState = new PatrolState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
        SearchState = new SearchState(this, StateMachine);
        //*******//
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desactivar rotación automática
        enemyAnimationController = GetComponentInChildren<EnemyAnimationController>();
        IsAttackingAI = false;
        ChasingPlayerAI = false;
        IsSearchingAI = false;
    }

    private void Start()
    {
        // Llamar periódicamente al sistema de decisión (en lugar de hacerlo cada frame)
        StateMachine.Initialize(IdleState);
    }
    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void ReiniciarNivel(int numPtosDentro)
    {
        PatrolState.ReiniciarNivel(numPtosDentro);
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
    public void actPosicPatrulla(int numPtsEliminar)
    {
        PatrolState.actPosicPatrulla(numPtsEliminar);
    }
    private void Update()
    {
        StateMachine.Update();
        if (ChasingPlayerAI)
        {
            enemyAnimationController.SetChasing(true);
        }
        else if (IsAttackingAI)
        {
            enemyAnimationController.PlayAttack();
        } else if (IsSearchingAI)
        {
            enemyAnimationController.PlaySearch();
        }
       
    }

    public void StopMovement()
    {

    }

    public void PlayAttackAnimation()
    {

    }

    public void LookAtPlayer()
    {

    }
    public void DoDamage()
    {

    }


    // --- Métodos que usarán los estados ---
    public bool CanSeePlayer() { return false; }
    public bool IsInAttackRange() { return false; }
    public void MoveTowardsPlayer() { }
}