public class EstadoPersecucion : EnemyState
{
    public EstadoPersecucion(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.agent.SetDestination(enemy.player.position);
        enemy.enemyAnimationController.Perseguir(true);// animacion
    }
    public override void Update()
    {
       
       
    }
}