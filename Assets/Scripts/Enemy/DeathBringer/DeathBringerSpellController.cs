using UnityEngine;

public class DeathBringerSpellController : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask playerLayer;

    private CharacterStat myStat;

    public void SetupSpell(CharacterStat _stat) => myStat = _stat;

    private void AnimationTrigger()
    {
        Collider2D[] collider = Physics2D.OverlapBoxAll(check.position, boxSize, playerLayer);

        foreach (var hit in collider)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockBackDir(transform);
                myStat.DoDamage(hit.GetComponent<CharacterStat>());
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);
    private void SelfDestroy() => Destroy(gameObject);
}
