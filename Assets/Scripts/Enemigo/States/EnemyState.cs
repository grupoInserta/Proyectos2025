public abstract class EnemyState
{
    protected EnemyAI enemy;
    protected StateMachine stateMachine;

    public EnemyState(EnemyAI enemy, StateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}