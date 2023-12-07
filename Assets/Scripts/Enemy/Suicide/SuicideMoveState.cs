public class SuicideMoveState : SuicideGroundState
{
    public SuicideMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Suicide _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }
    public override void Enter()
    {
        base.Enter();
        enemy.Flip();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();


        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.isWallDetected() || !enemy.isGroundDetected())
        {
            stateMachine.ChangeState(enemy.idleState);
        }

    }
}
