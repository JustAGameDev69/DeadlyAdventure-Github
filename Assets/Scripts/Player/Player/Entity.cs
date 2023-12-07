using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{

    [Header("Collision Check")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float groundCheckDistance = 1;
    [SerializeField] protected float wallCheckDistance = 0.8f;
    [SerializeField] protected LayerMask groundLayer;
    public Transform attackCheck;
    public float attackCheckRadius;

    [Header("KnockBack")]
    [SerializeField] protected Vector2 knockBackPower = new Vector2(7, 12);
    [SerializeField] protected Vector2 knockBackOffset = new Vector2(0.5f, 2);
    protected bool isKnocked;
    public float knockBackTime = 0.07f;

    #region Components Variable

    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }
    public CharacterStat stats { get; private set; }
    public CapsuleCollider2D capsuleCollider { get; private set; }

    #endregion

    public System.Action onFLipped;

    public int knockBackDirection { get; private set; }

    #region Flip
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;
    #endregion

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStat>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SLowEntityBy(float _percentage, float _duration)
    {

    }

    protected virtual void ReturnSpeed()
    {
        animator.speed = 1;
    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockBack");
    }

    public virtual void Death()
    {

    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    public virtual void SetupKnockBackDir(Transform damageDirection)
    {
        if (damageDirection.position.x > transform.position.x)          //damage come from right side
            knockBackDirection = -1;
        else if (damageDirection.position.x < transform.position.x)
            knockBackDirection = 1;
    }

    public void SetupKnockBackPower(Vector2 _knockBackPower) => knockBackPower = _knockBackPower;

    protected virtual IEnumerator HitKnockBack()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockBackOffset.x, knockBackOffset.y);

        rb.velocity = new Vector2((knockBackPower.x + xOffset) * knockBackDirection, knockBackPower.y);

        yield return new WaitForSeconds(knockBackTime);

        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    #region Set Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision Check
    public virtual bool isGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    public virtual bool isWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, groundLayer);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFLipped != null)
            onFLipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)           //Player are going to the right but not facing right --> flip
        {
            Flip();
        }
        else if (_x < 0 && facingRight)       //Opposite
        {
            Flip();

        }
    }

    public virtual void SetupDefaulFacingDir(int _direction)
    {
        facingDir = _direction;

        if (facingDir == -1)
            facingRight = false;
    }

    #endregion
}
