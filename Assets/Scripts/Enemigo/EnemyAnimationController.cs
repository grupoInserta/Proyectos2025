using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemy;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
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
        //
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        string stateName = GetStateName(stateInfo);

        Debug.Log("Estado actual: " + stateName);
    }

    private string GetStateName(AnimatorStateInfo stateInfo)
    {
        // Sustituye estos nombres por los de tus estados reales
        if (stateInfo.IsName("Idle")) return "Idle";
        if (stateInfo.IsName("Patrol")) return "Walk";
        if (stateInfo.IsName("Chase")) return "Run";
        if (stateInfo.IsName("Attack")) return "Attack";
        if (stateInfo.IsName("Search")) return "Search";
     

        return "Desconocido";
    }

    public void SetChasing(bool value)
    {
        animator.SetBool("IsChasing", value);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
        Debug.Log("ANIMACION ATACARRRRRRRRR");
    }

    public void PlaySearch()
    {
        animator.SetTrigger("Search");
    }   
}