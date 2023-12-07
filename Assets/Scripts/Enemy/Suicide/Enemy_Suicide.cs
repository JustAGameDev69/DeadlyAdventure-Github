using UnityEngine;

public class Enemy_Suicide : Enemy
{
    [Header("Suicide Info")]
    public float battleStateMoveSpeed;
    [SerializeField] private GameObject suicideExplodePrefab;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxSize;

    #region State
    public SuicideIdleState idleState { get; private set; }
    public SuicideMoveState moveState { get; private set; }
    public SuicideBattleState battleState { get; private set; }
    public SuicideStunState stunState { get; private set; }
    public SuicideDeadState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SuicideIdleState(this, stateMachine, "Idle", this);
        moveState = new SuicideMoveState(this, stateMachine, "Move", this);
        battleState = new SuicideBattleState(this, stateMachine, "MoveFast", this);
        stunState = new SuicideStunState(this, stateMachine, "Stunned", this);
        deathState = new SuicideDeadState(this, stateMachine, "Dead", this);

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunState);
            return true;
        }

        return false;
    }

    public override void Death()
    {
        base.Death();

        stateMachine.ChangeState(deathState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newExplosion = Instantiate(suicideExplodePrefab, attackCheck.position, Quaternion.identity);
        newExplosion.GetComponent<SuicideExplode_Controller>().SetupExplosion(stats, growSpeed, maxSize, attackCheckRadius);

        capsuleCollider.enabled = false;
        rb.gravityScale = 0;
    }

    public void SelfDestroy() => Destroy(gameObject);
}
