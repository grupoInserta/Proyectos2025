public class ChaseState : EnemyState
{
    public ChaseState(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Update()
    {
        enemy.MoveTowardsPlayer();

        if (!enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.SearchState);
        }
        else if (enemy.IsInAttackRange())
        {
            stateMachine.ChangeState(enemy.AttackState);
        }
    }
}