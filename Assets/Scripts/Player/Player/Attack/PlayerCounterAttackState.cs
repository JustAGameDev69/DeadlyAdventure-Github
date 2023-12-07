using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.animator.SetBool("SuccessCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        //Check if in the counter attack does our player hit any enemies. If we hit and enemy can be stunned => run success counter attack animation.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if(hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccessCounterAttack();
            }


            if (hit.GetComponent<Enemy>() != null)
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    SuccessCounterAttack();

                    player.skillManager.parry.UseSkill();

                    if (canCreateClone)
                    {
                        player.skillManager.parry.MakeCloneOnParry(hit.transform);     //An upgrade for counterAttack
                        canCreateClone = false;
                    }
                }
            }
        }

        //if we unsuccess with the counter attack => stateTime < 0 => exit state. If we success counter => trigger'll be called => exit state.

        if (stateTimer < 0 || triggerCall)
            stateMachine.ChangeState(player.idleState);

    }

    private void SuccessCounterAttack()
    {
        stateTimer = 10;
        player.animator.SetBool("SuccessCounterAttack", true);
    }
}
