using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform;             //Take the position of the sword when we cacth to know where's the sword, need Flip or not

        player.fx.PlayDustFX();
        player.fx.ScreenShake(player.fx.shakeSwordPower);

        if (player.transform.position.x > sword.position.x && player.facingDir == 1)
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1)
            player.Flip();

        rb.velocity = new Vector2(player.swordReturnForce * -player.facingDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyTime", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCall)
            stateMachine.ChangeState(player.idleState);
    }
}
