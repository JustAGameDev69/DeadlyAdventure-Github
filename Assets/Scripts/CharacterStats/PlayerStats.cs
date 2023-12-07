using UnityEngine;

public class PlayerStats : CharacterStat
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

    }

    protected override void Die()
    {
        base.Die();
        player.Death();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (_damage > GetMaxHealthValue() * 0.3f)
        {
            player.SetupKnockBackPower(new Vector2(5, 8));
            player.fx.ScreenShake(player.fx.shakeHighDamage);
            int randomSound = Random.Range(34, 35);
            AudioManager.instance.PlaySFX(randomSound, null);
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
        {
            currentArmor.Effect(player.transform);
        }
    }

    public override void OnEvasion()
    {
        player.skillManager.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStat _targetStat, float _multiplier)
    {
        if (CanAvoidAttack(_targetStat))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
        {
            totalDamage = CritPowerDamage(totalDamage);
        }

        totalDamage = TargetArmors(_targetStat, totalDamage);

        _targetStat.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStat);                                   //Comment this if not wanna do magical dmg on normal hit
    }
}
