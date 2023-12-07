using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter {  get; private set; }
    private float lastTimeAttacked;
    private float comboWindow = 2;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName) : base(_player, _stateMachine, _aniBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0;         //Fix Attack Bug

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        player.animator.SetInteger("ComboCounter", comboCounter);

        float attackDir = player.facingDir;                 //Choose attack direction

        if(xInput != 0)
            attackDir = xInput;

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);      //Add some movement for the attack (better fight)



        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyTime", 0.15f);

        comboCounter++;

        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        if (triggerCall)
            stateMachine.ChangeState(player.idleState);
    }
}
