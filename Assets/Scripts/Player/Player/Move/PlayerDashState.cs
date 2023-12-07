public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skillManager.dash.CloneOnDash();

        stateTimer = player.dashDuration;

        player.stats.MakeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.skillManager.dash.CloneOnArrival();
        player.SetVelocity(0, rb.velocity.y);
        player.stats.MakeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (!player.isGroundDetected() && player.isWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        player.fx.CreateAfterImage();

    }
}
