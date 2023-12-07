using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;
    private float defaultGravity;

    public PlayerBlackHoleState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void AnimationEndTrigger()
    {
        base.AnimationEndTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.fx.MakeTransprent(false);

    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }

        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -0.1f);

            if (!skillUsed)
            {
                if(player.skillManager.blackHole.CanUseSkill())
                    skillUsed = true;
            }
        }

        if (player.skillManager.blackHole.SkillFinished())
            stateMachine.ChangeState(player.airState);
    }
}
