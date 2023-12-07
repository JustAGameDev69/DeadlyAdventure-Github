using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected float xInput;
    protected float yInput;
    protected Rigidbody2D rb;

    private string aniBoolName;

    protected float stateTimer;
    protected bool triggerCall;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _aniBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.aniBoolName = _aniBoolName;
    }

    public virtual void Enter()
    {
        player.animator.SetBool(aniBoolName, true);
        rb = player.rb;

        triggerCall = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");


        player.animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        player.animator.SetBool(aniBoolName, false);
    }

    public virtual void AnimationEndTrigger()
    {
        triggerCall = true;
    }

}
