using UnityEngine;

public class EnemyState
{

    protected EnemyStateMachine stateMachine;

    protected Enemy enemyBase;

    protected bool triggerCall;
    protected float stateTimer;
    protected Rigidbody2D rb;

    private string animBoolName;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Enter()
    {
        triggerCall = false;
        rb = enemyBase.rb;
        enemyBase.animator.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        enemyBase.animator.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationEndTrigger()
    {
        triggerCall = true;
    }

}
