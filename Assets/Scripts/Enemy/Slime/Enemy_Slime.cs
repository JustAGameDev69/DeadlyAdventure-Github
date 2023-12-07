using UnityEngine;

public enum SlimeType { big, medium, small }

public class Enemy_Slime : Enemy
{
    [Header("Slime specific")]
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int SlimeToDuplicate;
    [SerializeField] private GameObject slimePrefabs;
    [SerializeField] private Vector2 minDuplicateVelocity;
    [SerializeField] private Vector2 maxDuplicateVelocity;

    #region States
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunState stunState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetupDefaulFacingDir(-1);

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunState = new SlimeStunState(this, stateMachine, "Stunned", this);
        deadState = new SlimeDeadState(this, stateMachine, "Idle", this);
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

        stateMachine.ChangeState(deadState);

        if (slimeType == SlimeType.small)
            return;

        CreateSlimeOnDie(SlimeToDuplicate, slimePrefabs);

    }

    private void CreateSlimeOnDie(int _amountOfSlimes, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimes; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
            newSlime.GetComponent<Enemy_Slime>().SetupSlime(facingDir);
        }
    }

    public void SetupSlime(int _facingDir)
    {
        if (_facingDir != facingDir)
            Flip();

        float xVelocity = Random.Range(minDuplicateVelocity.x, maxDuplicateVelocity.x);
        float yVelocity = Random.Range(minDuplicateVelocity.y, maxDuplicateVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDir, yVelocity);

        Invoke("CancelKnockBack", 1.5f);
    }

    private void CancelKnockBack() => isKnocked = false;

}
