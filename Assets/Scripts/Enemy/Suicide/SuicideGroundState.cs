using UnityEngine;

public class SuicideGroundState : EnemyState
{
    private Transform player;
    protected Enemy_Suicide enemy;

    public SuicideGroundState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Suicide _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < enemy.agressiveDistance)
            stateMachine.ChangeState(enemy.battleState);
    }

}
