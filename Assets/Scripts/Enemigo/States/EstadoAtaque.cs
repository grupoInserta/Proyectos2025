using UnityEngine;
using System.Collections;
public class EstadoAtaque : EnemyState
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public EstadoAtaque(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.enemyAnimationController.Atacar();
        /**** enemyAnimationController.PlayAttack();***/
       
    }
    
}