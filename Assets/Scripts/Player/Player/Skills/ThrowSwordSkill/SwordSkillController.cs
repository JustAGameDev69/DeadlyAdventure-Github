using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;
    private float freezeDuration;

    [Header("Bounce Sword Setup")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Pierce Sword Setup")]
    private int pierceAmount;

    [Header("Spin Sword Setup")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStop;
    private bool isSpining;
    //private float spinDirection;                                  [MAX LEVEL SKILL]

    private float hitTimer;
    private float hitCooldown;

    [SerializeField] private float returnSpeed = 12;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeDuration)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeDuration = _freezeDuration;

        if (pierceAmount <= 0)
            animator.SetBool("Rotation", true);

        //spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);        [MAX LEVEL SKILL]

        Invoke("DestroySword", 10);
    }

    private void DestroySword()
    {
        Destroy(gameObject);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;

        enemyTarget = new List<Transform>();
    }

    public void SetupSpin(bool _isSpining, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpining = _isSpining;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;

    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            animator.SetBool("Rotation", true);

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchSword();
        }

        BounceSwordLogic();

        SpinningLogic();
    }

    private void SpinningLogic()
    {
        if (isSpining)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStop)
            {
                StopSpinning();
            }

            if (wasStop)
            {
                spinTimer -= Time.deltaTime;
                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                //This is an too powerfull skill should only be unlock at max level  [MAX LEVEL SKILL]

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpining = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    private void StopSpinning()
    {
        wasStop = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceSwordLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }


    private void SetupBounceTarget(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)           //Check for enemy around the collision, if there're some enemies, get them position
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }
    }


    #region SwordCollision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);
        }

        SetupBounceTarget(collision);

        StuckInTo(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        player.stats.DoDamage(enemyStats);

        if(player.skillManager.throwSword.timeSwordUnlocked)
            enemy.FreezeTimeFor(freezeDuration);

        if (player.skillManager.throwSword.timeSwordUpgradeUnlocked)
            enemyStats.MakeVulnerableFor(freezeDuration);

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equipedAmulet != null)
            equipedAmulet.Effect(enemy.transform);
    }

    private void StuckInTo(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpining)
        {
            StopSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0)
            return;

        animator.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
    #endregion
}
