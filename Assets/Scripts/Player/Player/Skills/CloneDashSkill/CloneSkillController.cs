using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator animator;
    [SerializeField] private float loosingColorSpeed;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = 0.8f;
    private Transform closestEnemy;
    private bool canDuplicateClone;
    private int facingDirection = 1;
    private float chanceToDuplicate;
    private float attackMultiplier;

    private float cloneTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        
        if(cloneTimer < 0)
        {
            sr.color = new Color(1,1,1,sr.color.a - (Time.deltaTime* loosingColorSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }

    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offSet, Transform _closestEnemy, bool _canDuplicateClone, float _chanceToDuplicate, Player _player, float _attackMultiplier)
    {
        if (_canAttack)
            animator.SetInteger("AttackNumber", Random.Range(1, 3));

        player = _player;
        transform.position = _newTransform.position + _offSet;
        cloneTimer = _cloneDuration;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        attackMultiplier = _attackMultiplier;

        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;                 //End of the attack, clone disappear
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStat>());

                hit.GetComponent<Entity>().SetupKnockBackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if(player.skillManager.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)                      //Ultimate powerful skill (Unlock lastest - expensive)
                {
                    if(Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1f * facingDirection, 0));       
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {

        if(closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)                 //Check if enemy on the left side, then rotate
            {
                facingDirection = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
