using UnityEngine;

public class ArcherBattleState : EnemyState
{
    private Transform player;
    private Enemy_Archer enemy;
    private int moveDir;

    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.safeDistance)
            {
                if (CanJump())
                    stateMachine.ChangeState(enemy.jumpState);

            }

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance && CanAttack())
            {
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 5)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
            enemy.Flip();

    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastAttackTime + enemy.minAttackCooldown)        //Example: lastattackTime at 15s + 2s cooldown => at 17s in game skeleton could attack again
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastAttackTime = Time.time;                               //Example: lastTimeAttack = 17s now
            return true;
        }
        return false;
    }

    private bool CanJump()
    {
        if (!enemy.GroundBehindCheck() && enemy.WallBehindCheck())
            return false;

        if (Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown)
        {
            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;
    }
}
