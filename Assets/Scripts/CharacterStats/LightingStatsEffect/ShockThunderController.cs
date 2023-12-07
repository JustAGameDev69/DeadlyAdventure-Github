using UnityEngine;

public class ShockThunderController : MonoBehaviour
{
    [SerializeField] private CharacterStat targetStats;
    [SerializeField] private float speed;
    private bool trigger;
    private int damage;

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!targetStats)
            return;

        if (trigger)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.1f)
        {
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);
            anim.transform.localPosition = new Vector3(0, 0.45f, 0);

            Invoke("DamageAndDestroy", 0.2f);
            trigger = true;
            anim.SetTrigger("Hit");
        }
    }

    public void SetupThunder(int _damage, CharacterStat _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    private void DamageAndDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, 0.4f);
    }
}
