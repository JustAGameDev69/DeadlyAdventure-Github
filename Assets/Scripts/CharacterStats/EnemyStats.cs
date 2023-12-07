using UnityEngine;
public class EnemyStats : CharacterStat
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stats soulsDropAmount;

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifiers = 0.2f;


    protected override void Start()
    {
        soulsDropAmount.SetDefaulValue(100);
        LevelModify();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();

    }

    private void LevelModify()
    {
        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHP);
        Modify(armor);
        Modify(evasion);
        Modify(MagicResistant);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);

        Modify(soulsDropAmount);
    }

    private void Modify(Stats _stat)
    {
        for (int i = 1; i <= level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifiers;
            _stat.AddModifiers(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

    }

    protected override void Die()
    {
        base.Die();

        enemy.Death();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();

        Destroy(gameObject, 3f);
    }
}
