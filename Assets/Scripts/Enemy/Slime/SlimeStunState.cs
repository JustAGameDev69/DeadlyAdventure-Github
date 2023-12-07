using UnityEngine;

public class SlimeStunState : EnemyState
{
    private Enemy_Slime enemy;
    public SlimeStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.stunDuration;

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        if (rb.velocity.y < 0.1f && enemy.isGroundDetected())
        {
            enemy.fx.Invoke("CancelColorChange", 0);
            enemy.animator.SetTrigger("StunHold");
            enemy.stats.MakeInvincible(true);
        }

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
