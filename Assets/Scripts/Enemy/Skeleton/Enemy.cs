using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask playerLayer;

    [Header("Move")]
    public float moveSpeed = 2.5f;
    public float idleTime = 1.75f;
    public float battleTime = 10;
    private float defaultMoveSpeed;

    [Header("Attack")]
    public float playerDetectedRange = 10;
    public float agressiveDistance = 2;
    public float attackDistance = 1.35f;
    public float attackCooldown;
    public float maxAttackCooldown = 0.35f;
    public float minAttackCooldown = 0.55f;
    [HideInInspector] public float lastAttackTime;

    [Header("Stun")]
    public float stunDuration = 1;
    public Vector2 stunDirection = new Vector2(10, 12);
    private bool canBeStunned;
    [SerializeField] private GameObject counterImg;

    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName { get; private set; }
    public EntityFX fx { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defaultMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponent<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationEndTrigger();
    public virtual void AnimationSpecialAttackTrigger()
    {

    }
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, playerDetectedRange, playerLayer);


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    public override void SLowEntityBy(float _percentage, float _duration)
    {
        moveSpeed = moveSpeed * (1 - _percentage);
        animator.speed = animator.speed * (1 - _percentage);
        Invoke("ReturnSpeed", _duration);
    }

    protected override void ReturnSpeed()
    {
        base.ReturnSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void AssignLastAnimName(string _aniBoolName)
    {
        lastAnimBoolName = _aniBoolName;
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreeTimeCoroutine(_duration));

    public virtual void FreezeTime(bool _timerFreeze)
    {
        if (_timerFreeze)
        {
            moveSpeed = 0;
            animator.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            animator.speed = 1;
        }
    }

    protected virtual IEnumerator FreeTimeCoroutine(float _second)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_second);

        FreezeTime(false);
    }

    #region CounterAttack
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImg.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImg.SetActive(false);
    }
    #endregion
}
