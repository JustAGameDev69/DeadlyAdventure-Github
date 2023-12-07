using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    private float crystalExistTimer;
    private bool canExplode;
    private bool canMoveToEnemy;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;
    private Transform closestTarget;
    [SerializeField] private LayerMask enemyLayer;
    private Player player;

    private Animator animator => GetComponent<Animator>();
    private CircleCollider2D circleCollider => GetComponent<CircleCollider2D>();

    public void SetupCrystal(float _duration, bool _canExplode, bool _canMoveToEnemy, float _moveSpeed, Transform _closestTarget, Player _player)
    {
        player = _player;
        crystalExistTimer = _duration;
        canExplode = _canExplode;
        canMoveToEnemy = _canMoveToEnemy;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void Update()
    {

        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            CrystalExplode();
        }

        if (closestTarget == null)
            return;

        if (canMoveToEnemy)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 0.8f)
            {
                CrystalExplode();
                canMoveToEnemy = false;
            }

        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(2, 2), growSpeed * Time.deltaTime);
        }
    }

    public void ChooseRandomEnemies()               //Upgrade for crystal instead of clone
    {
        float radius = SkillManager.instance.blackHole.GetBlackHoleRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,radius, enemyLayer);

        if(colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    public void CrystalExplode()
    {
        if (canExplode)
        {
            canGrow = true;
            animator.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    private void AnimatorEventForExplode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockBackDir(transform);
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStat>());

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
                if (equipedAmulet != null)
                    equipedAmulet.Effect(hit.transform);
            }
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}
