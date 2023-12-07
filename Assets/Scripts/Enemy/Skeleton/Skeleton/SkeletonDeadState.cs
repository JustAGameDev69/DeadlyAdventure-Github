using UnityEngine;

public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy_Skeleton) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy_Skeleton;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.animator.SetBool(enemy.lastAnimBoolName, true);
        enemy.animator.speed = 0;
        enemy.capsuleCollider.enabled = false;
        stateTimer = 0.2f;
    }


    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 6);
        }
    }
}
