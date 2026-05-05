using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemy;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float speed = enemy.agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        Debug.Log("SPEED: " + speed);
    }

    public void SetChasing(bool value)
    {
        animator.SetBool("IsChasing", value);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void PlaySearch()
    {
        animator.SetTrigger("Search");
    }   
}