using UnityEngine;

public class SuicideExplode_Controller : MonoBehaviour
{
    private Animator animator;
    private CharacterStat myStat;
    private float growSpeed;
    private float maxSize;
    private float explosionRadius;
    private bool canGrow = true;

    private void Update()
    {
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        if (maxSize - transform.localScale.x < 0.5f)
        {
            canGrow = false;
            animator.SetTrigger("Explode");
        }
    }

    public void SetupExplosion(CharacterStat _myStat, float _growSpeed, float _maxSize, float _radius)
    {
        animator = GetComponent<Animator>();
        myStat = _myStat;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _radius;
    }

    private void AnimatorEventForExplode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStat>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockBackDir(transform);
                myStat.DoDamage(hit.GetComponent<CharacterStat>());
            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);
}
