using UnityEngine;

public class EstadoBusqueda : EnemyState
{
    private float searchDuration = 4f;
    private float timer;
    private float rotationSpeed = 120f; // grados por segundo
    private float changeDirectionTime = 1.5f;
    private float directionTimer;
    private float currentDirection = 1f;

    public EstadoBusqueda(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        // Opcional: animación de alerta
        // enemy.PlaySearchAnimation();
        timer = searchDuration;
        directionTimer = changeDirectionTime;
        // Detener movimiento mientras busca
       // enemy.StopMovement();
    }


    public override void Update()
    {
        // Si vuelve a ver al jugador → persecución
        if (enemy.PuedeVerAlJugador)
        {
            stateMachine.ChangeState(enemy.estadoBusqueda);
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
            stateMachine.ChangeState(enemy.estadoPatrulla);
        }
    }
    
}