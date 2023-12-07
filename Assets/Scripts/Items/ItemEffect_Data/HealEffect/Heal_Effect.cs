using UnityEngine;

[CreateAssetMenu(fileName = "Heal Data", menuName = "Data/Item effect/Heal Effect")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);

        playerStats.IncreaseHealthBy(healAmount);
    }
}
