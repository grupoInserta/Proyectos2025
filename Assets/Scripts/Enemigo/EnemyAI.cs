using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public EstadoPausa estadoPausa { get; private set; }
    public EstadoPatrulla estadoPatrulla { get; private set; }
    public EstadoPersecucion estadoPersecucion { get; private set; }
    public EstadoAtaque estadoAtaque { get; private set; }
    public EstadoBusqueda estadoBusqueda { get; private set; }
    //
    public Transform player;
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public Transform[] patrolPointsReinicio;// puntos para reiniciar la patrulla en cada nivel

    public float rotationSpeed = 8f;
    public bool PuedeVerAlJugador = false;
    public EnemyAnimationController enemyAnimationController;
    // para las pausas:
    public int currentPatrolIndex = -1;
    public float pauseDuration = 4f;
    // sonidos
    public AudioSource audioSource2;
    public AudioClip PasosEnemigo;
 
    //
    [Header("Distancia")]
    public float maxDistance = 20f;

    [Header("Volumen")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;


    private void Awake()
    {
        StateMachine = new StateMachine();

        estadoPausa = new EstadoPausa(this, StateMachine); // INICIAL
        estadoPatrulla = new EstadoPatrulla(this, StateMachine);
        estadoBusqueda = new EstadoBusqueda(this, StateMachine);
        estadoAtaque = new EstadoAtaque(this, StateMachine);
        estadoPersecucion = new EstadoPersecucion(this, StateMachine);
       
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desactivar rotación automática
        enemyAnimationController = GetComponentInChildren<EnemyAnimationController>();
    }

    private void Start()
    {
        // Llamar periódicamente al sistema de decisión (en lugar de hacerlo cada frame)
        StateMachine.Initialize(estadoPatrulla);
    }
    public Coroutine RunCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void ReiniciarNivel(int numPtosDentro)
    {
        estadoPatrulla.ReiniciarNivel(numPtosDentro);
        Debug.Log("REINICIAR NIVEL");
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
        estadoPatrulla.actPosicPatrulla(numPtsEliminar);
    }

    // ************** UTILIZAR ChangeState para cualquier estado: StateMachine.ChangeState(EnemyState newState)
    private void Update() // SOLO ANIMACIONES  !!!!!!!!!!!!!!!!
    {
        StateMachine.Update();
        SonidoPasos();
        Debug.Log("SUENA??????"+audioSource2.isPlaying);
    }
   

    public void Accion(string accion)
    {
        if(accion == "Atacar") {
            StateMachine.ChangeState(estadoAtaque);
        } 
        else if(accion == "Patrullar")
        {
            StateMachine.ChangeState(estadoPatrulla);            
        }
        else if (accion == "Perseguir")
        {
            StateMachine.ChangeState(estadoPersecucion);            
        }
        else if (accion == "Buscar")
        {
            StateMachine.ChangeState(estadoBusqueda);
        }
        else if (accion == "Parar")
        {
            StateMachine.ChangeState(estadoPausa);
        }

    }

    public void PararSoniPasos()
    {
        audioSource2.Stop();
    }

    private void SonidoPasos()
    {
        if (player == null || PasosEnemigo == null)
            return;       
        float distance = Vector3.Distance(transform.position, player.position);

        // volumen inverso según distancia
        float volume = 1f - Mathf.Clamp01(distance / maxDistance);
        audioSource2.volume = volume * maxVolume;
    }

    public void ReproducirSoniPasos()
    {
        audioSource2.clip = PasosEnemigo;
        audioSource2.loop = true;
        audioSource2.Play();
       
    }

    public void Buscar()
    {
        StateMachine.ChangeState(estadoBusqueda);
        enemyAnimationController.Buscar();
    }

    public void Parar()
    {
        StateMachine.ChangeState(estadoPausa);
        enemyAnimationController.Parar();
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
 
    public bool IsInAttackRange() { return false; }
    public void MoveTowardsPlayer() { }
}