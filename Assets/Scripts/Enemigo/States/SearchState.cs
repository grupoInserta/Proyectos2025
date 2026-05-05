using UnityEngine;

public class SearchState : EnemyState
{
    private float searchDuration = 4f;
    private float timer;
    private float rotationSpeed = 120f; // grados por segundo
    private float changeDirectionTime = 1.5f;
    private float directionTimer;
    private float currentDirection = 1f;

    public SearchState(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        // Opcional: animación de alerta
        // enemy.PlaySearchAnimation();
        timer = searchDuration;
        directionTimer = changeDirectionTime;
        // Detener movimiento mientras busca
        enemy.StopMovement();
    }


    public override void Update()
    {
        // Si vuelve a ver al jugador → persecución
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0)
        {
            currentDirection *= -1f; // cambia sentido
            directionTimer = changeDirectionTime;
        }

        enemy.transform.Rotate(0f, currentDirection * rotationSpeed * Time.deltaTime, 0f);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            stateMachine.ChangeState(enemy.PatrolState);
        }

    }

    
}