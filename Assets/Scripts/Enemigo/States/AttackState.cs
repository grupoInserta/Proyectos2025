using UnityEngine;
using System.Collections;
public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public AttackState(EnemyAI enemy, StateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter()
    {

       /**** enemyAnimationController.PlayAttack();***/
        lastAttackTime = Time.time;

        // Opcional: detener movimiento
        enemy.StopMovement();

        // Opcional: activar animación
        enemy.PlayAttackAnimation();
    }

    public override void Update()
    {
        // Mirar al jugador constantemente
        enemy.LookAtPlayer();

        // Si el jugador se aleja → perseguir
        if (!enemy.IsInAttackRange())
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // Atacar con cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        // Lógica de daño
        enemy.DoDamage();

        // Animación
        enemy.PlayAttackAnimation();
    }
}