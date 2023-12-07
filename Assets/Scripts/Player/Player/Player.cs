using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("Basic Actions")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float jumpForce;
    //For chill effect
    private float defaulMoveSpeed;
    private float defaulJumpForce;

    [Header("Dash Ability")]
    public float dashSpeed;
    public float dashDuration;
    private float defaulDashSpeed;      //For chill effect
    public float dashDir { get; private set; }

    [Header("Attack Stats")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.15f;

    public float swordReturnForce;

    public SkillManager skillManager { get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }

    public bool isBusy { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }                        //Like Read-Only 
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackHoleState blackHole { get; private set; }
    public PlayerDeathState deathState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        deathState = new PlayerDeathState(this, stateMachine, "Die");
        //---------------------------------------------------------------------------------------------------
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        //---------------------------------------------------------------------------------------------------
        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackHoleState(this, stateMachine, "Jump");
    }

    protected override void Start()
    {
        base.Start();

        defaulJumpForce = jumpForce;
        defaulMoveSpeed = moveSpeed;
        defaulDashSpeed = dashSpeed;

        fx = GetComponent<PlayerFX>();
        skillManager = SkillManager.instance;

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();


        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F) && skillManager.crystal.crystalUnlocked)
            skillManager.crystal.CanUseSkill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Inventory.instance.UseFlask();
        }
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationEndTrigger();

    public override void SLowEntityBy(float _percentage, float _duration)
    {
        moveSpeed = moveSpeed * (1 - _percentage);
        jumpForce = jumpForce * (1 - _percentage);
        dashSpeed = dashSpeed * (1 - _percentage);

        animator.speed = animator.speed * (1 - _percentage);

        Invoke("ReturnSpeed", _duration);
    }

    protected override void ReturnSpeed()
    {
        base.ReturnSpeed();

        moveSpeed = defaulMoveSpeed;
        jumpForce = defaulJumpForce;
        dashSpeed = defaulDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public override void Death()
    {
        base.Death();

        stateMachine.ChangeState(deathState);
    }

    private void CheckForDashInput()
    {
        if (isWallDetected())
            return;

        if (!skillManager.dash.dashUnlocked)
            return;

        if (Input.GetKeyDown(KeyCode.E) && SkillManager.instance.dash.CanUseSkill())
        {

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
            {
                dashDir = facingDir;
            }

            stateMachine.ChangeState(dashState);
        }
    }

    public IEnumerator BusyTime(float _second)                   //Get some delay
    {
        isBusy = true;

        yield return new WaitForSeconds(_second);

        isBusy = false;
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockBackPower = new Vector2(0, 0);
    }

}
