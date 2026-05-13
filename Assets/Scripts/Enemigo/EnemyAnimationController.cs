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
        if (stateInfo.IsName("Pausa")) return "Idle";
        if (stateInfo.IsName("Patrulla")) return "Walk";
        if (stateInfo.IsName("Persecucion")) return "Run";
        if (stateInfo.IsName("Ataque")) return "Attack";
        if (stateInfo.IsName("Busqueda")) return "Search";
        return "Desconocido";
    }
   

    public void Parar()
    {
        animator.SetFloat("Speed", 0);
    }

    public void Perseguir(bool value)
    {       
        animator.SetBool("Persiguiendo", value);
    }

    public void Atacar()
    {
        animator.SetTrigger("Atacar");
    }

    public void Buscar()
    {
        animator.SetTrigger("Busqueda");
    } 
    
    public void Patrullar()
    {
        animator.SetTrigger("Patrulla");
    }
}