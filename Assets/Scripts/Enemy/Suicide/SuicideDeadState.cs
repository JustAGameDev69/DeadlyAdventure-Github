public class SuicideDeadState : EnemyState
{
    private Enemy_Suicide enemy;
    public SuicideDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Suicide _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

    }


    public override void Update()
    {
        base.Update();

        if (triggerCall)
            enemy.SelfDestroy();
    }
}
