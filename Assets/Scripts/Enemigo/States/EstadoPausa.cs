using UnityEngine;
public class EstadoPausa : EnemyState
{
   
    public EstadoPausa(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    private float timer;

    public override void Enter()
    {        
        timer = enemy.pauseDuration;
        enemy.agent.isStopped = true;
        // animación idle
        enemy.enemyAnimationController.Parar();  
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            enemy.Accion("Patrullar");
        }
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
    }
}




    


