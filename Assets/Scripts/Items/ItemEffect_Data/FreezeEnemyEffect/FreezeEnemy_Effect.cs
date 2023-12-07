using UnityEngine;

[CreateAssetMenu(fileName = "Freeze effect", menuName = "Data/Item effect/Freeze Enemy")]
public class FreezeEnemy_Effect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats.currentHP > playerStats.GetMaxHealthValue() * 0.1f)
            return;

        if (!Inventory.instance.CanUseArmorEffect())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);

        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
    }
}
