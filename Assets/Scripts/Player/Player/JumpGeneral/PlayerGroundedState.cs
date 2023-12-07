using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!player.isGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if (Input.GetKeyDown(KeyCode.R) && player.skillManager.blackHole.blackHoleUnlocked)
        {
            if(player.skillManager.blackHole.skillCooldown > 0)
            {
                player.fx.CreatePopupText("On cooldown!");
                return;
            }

            stateMachine.ChangeState(player.blackHole);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skillManager.throwSword.swordSkillUnlocked)           //Check if player already throw a sword but didn't take it back yet
            stateMachine.ChangeState(player.aimSword);

        if (Input.GetKeyDown(KeyCode.Q) && player.skillManager.parry.parryUnlocked)
            stateMachine.ChangeState(player.counterAttack);


        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        if (Input.GetKeyDown(KeyCode.Space) && player.isGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
