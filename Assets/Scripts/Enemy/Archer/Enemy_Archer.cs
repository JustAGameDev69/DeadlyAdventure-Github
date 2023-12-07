using UnityEngine;

public class Enemy_Archer : Enemy
{
    [Header("Archer Info")]
    [SerializeField] public Vector2 jumpVelocity;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float arrowDamage;
    public float jumpCooldown;
    public float safeDistance;
    [HideInInspector] public float lastTimeJumped;

    [Header("Jump Collision Check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    #region State
    public ArcherIdleState idleState { get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherStunState stunState { get; private set; }
    public ArcherDeadState deathState { get; private set; }
    public ArcherJumpState jumpState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        stunState = new ArcherStunState(this, stateMachine, "Stunned", this);
        deathState = new ArcherDeadState(this, stateMachine, "Idle", this);
        jumpState = new ArcherJumpState(this, stateMachine, "Jump", this);

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
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

    private Quaternion SetupRotation()
    {
        if (facingDir == -1)
            return Quaternion.Euler(0, 180, 0);
        else
            return Quaternion.Euler(0, 0, 0);
    }


    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, SetupRotation());
        newArrow.GetComponent<Arrow_Controller>().SetupArrow(arrowSpeed * facingDir, stats);
    }

    public bool GroundBehindCheck() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, groundLayer);
    public bool WallBehindCheck() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, groundLayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }

}
