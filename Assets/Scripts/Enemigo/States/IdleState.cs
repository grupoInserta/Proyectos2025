using UnityEngine;
public class IdleState : EnemyState
{
    private int contador = 0;
    public IdleState(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    //
    public override void Update()
    {
        if(contador == 20) stateMachine.ChangeState(enemy.PatrolState);
        contador++;
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.ChaseState);
        }
    }


}