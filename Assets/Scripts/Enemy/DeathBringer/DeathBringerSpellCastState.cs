using UnityEngine;

public class DeathBringerSpellCastState : EnemyState
{
    private Enemy_DeathBringer enemy;

    private int amountOfSpell;
    private float spellTimer;

    public DeathBringerSpellCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        amountOfSpell = enemy.amountOfSpell;
        spellTimer = 0.5f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeCast = Time.time;
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        if (CanCast())
        {
            enemy.CastSpell();
        }


        if (amountOfSpell <= 0)
            stateMachine.ChangeState(enemy.teleportState);
    }

    private bool CanCast()
    {
        if (amountOfSpell > 0 && spellTimer < 0)
        {
            amountOfSpell--;
            spellTimer = enemy.spellCooldown;
            return true;
        }

        else return false;
    }
}
