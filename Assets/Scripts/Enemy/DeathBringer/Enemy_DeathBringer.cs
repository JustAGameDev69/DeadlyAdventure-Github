using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public bool bossFightStart;

    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D area;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    public float defaultChanceToTeleport;

    [Header("Spell Cast")]
    [SerializeField] private GameObject spellCastPrefab;
    [SerializeField] private float spellCastStateCooldown;
    public int amountOfSpell;
    public float spellCooldown;
    public float lastTimeCast;

    #region State
    public DeathBringerIdleState idleState { get; private set; }
    public DeathBringerTeleportState teleportState { get; private set; }
    public DeathBringerBattleState battleState { get; private set; }
    public DeathBringerAttackState attackState { get; private set; }
    public DeathBringerSpellCastState spellcastState { get; private set; }
    public DeathBringerDeadState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetupDefaulFacingDir(-1);

        idleState = new DeathBringerIdleState(this, stateMachine, "Idle", this);
        teleportState = new DeathBringerTeleportState(this, stateMachine, "Teleport", this);
        battleState = new DeathBringerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringerAttackState(this, stateMachine, "Attack", this);
        spellcastState = new DeathBringerSpellCastState(this, stateMachine, "Cast", this);
        deathState = new DeathBringerDeadState(this, stateMachine, "Idle", this);

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

    public override void Death()
    {
        base.Death();

        stateMachine.ChangeState(deathState);
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.velocity.x != 0)             //Player's moving
            xOffset = player.facingDir * 3;

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + 1.48f);

        GameObject newSpell = Instantiate(spellCastPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringerSpellController>().SetupSpell(stats);
    }

    public void FindPosition()
    {
        float x = Random.Range(area.bounds.min.x + 3, area.bounds.max.x - 3);
        float y = Random.Range(area.bounds.min.y + 3, area.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (capsuleCollider.size.y / 2));

        if (!GroundBelow() || SomethingAtTelePosCheck())
        {
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, groundLayer);
    private bool SomethingAtTelePosCheck() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, groundLayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellCastStateCooldown)
        {
            return true;
        }

        return false;
    }

}
