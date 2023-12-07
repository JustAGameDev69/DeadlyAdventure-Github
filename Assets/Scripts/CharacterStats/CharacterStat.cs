using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    intelligence,
    agility,
    vitality,
    health,
    damage,
    critChance,
    critPower,
    armor,
    evasion,
    MagicResistant,
    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStat : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stats strength;          //increase damage and crit power
    public Stats intelligence;      //increase magic damage and 3 magic resistant
    public Stats agility;           //increase evasion and crit chance
    public Stats vitality;          //increase health

    [Header("Defensive Stats")]
    public Stats maxHP;
    public int currentHP;
    public Stats armor;
    public Stats evasion;
    public Stats MagicResistant;

    [Header("Offensice Stats")]
    public Stats damage;
    public Stats critChance;
    public Stats critPower;

    [Header("Magic Stats")]
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightingDamage;

    public bool isIgnite;           //Does damage in a while
    public bool isChilled;          //Reduce target armor
    public bool isShocked;          //Reduce accuracy 

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCoolDown = 0.3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefabs;
    private int shockDamage;

    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    public System.Action onHealthChange;

    protected virtual void Start()
    {
        critPower.SetDefaulValue(150);
        currentHP = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnite = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnite)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCorountine(_duration));
    }

    private IEnumerator VulnerableCorountine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stats _statToModify)
    {
        StartCoroutine(StartModifyCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StartModifyCoroutine(int _modifier, float _duration, Stats _statToModify)
    {
        _statToModify.AddModifiers(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifiers(_modifier);
    }

    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }
    public void DoDamage(CharacterStat _targetStat)
    {
        bool criticalStrike = false;

        if (_targetStat.isInvincible)
            return;

        if (CanAvoidAttack(_targetStat))
            return;

        _targetStat.GetComponent<Entity>().SetupKnockBackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CritPowerDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStat.transform, criticalStrike);

        totalDamage = TargetArmors(_targetStat, totalDamage);

        _targetStat.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStat);                                   //Comment this if not wanna do magical dmg on normal hit
    }
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHP -= _damage;

        if (_damage > 0)
            fx.CreatePopupText(_damage.ToString());

        if (onHealthChange != null)
            onHealthChange();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHP += _amount;

        if (currentHP > GetMaxHealthValue())
        {
            currentHP = GetMaxHealthValue();
        }

        if (onHealthChange != null)
            onHealthChange();
    }

    #region Do Magic Damage

    public virtual void DoMagicalDamage(CharacterStat _targetStat)
    {
        int fireDmg = fireDamage.GetValue();
        int iceDmg = iceDamage.GetValue();
        int lightDmg = lightingDamage.GetValue();

        int totalMagicDamage = fireDmg + iceDmg + lightDmg + intelligence.GetValue();
        totalMagicDamage = GetTargetMagicResistant(_targetStat, totalMagicDamage);

        _targetStat.TakeDamage(totalMagicDamage);

        if (Mathf.Max(fireDmg, iceDmg, lightDmg) <= 0)
            return;

        AttemptApplyElements(_targetStat, fireDmg, iceDmg, lightDmg);

    }

    private void AttemptApplyElements(CharacterStat _targetStat, int fireDmg, int iceDmg, int lightDmg)
    {
        bool canApplyIgnite = fireDmg > iceDmg && fireDmg > lightDmg;
        bool canApplyChill = iceDmg > fireDmg && iceDmg > lightDmg;
        bool canApplyShock = lightDmg > fireDmg && lightDmg > iceDmg;


        while (!canApplyIgnite && !canApplyChill && !canApplyShock)              //Happened when player has the same stats
        {
            if (Random.value < 0.5f && fireDmg > 0)
            {
                canApplyIgnite = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && iceDmg > 0)
            {
                canApplyChill = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < 0.5f && lightDmg > 0)
            {
                canApplyShock = true;
                _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStat.SetupIgniteDamage(Mathf.RoundToInt(fireDmg * 0.2f));

        if (canApplyShock)
            _targetStat.SetupShockStrikeDamage(Mathf.RoundToInt(lightDmg * 0.1f));

        _targetStat.ApplyAliments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private int GetTargetMagicResistant(CharacterStat _targetStat, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStat.MagicResistant.GetValue() + (_targetStat.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public void ApplyAliments(bool _isIgnite, bool _isChilled, bool _isShocked)
    {
        bool canApplyIgnite = !isIgnite && !isChilled && !isShocked;
        bool canApplyChill = !isIgnite && !isChilled && !isShocked;
        bool canApplyShock = !isIgnite && !isChilled;

        if (_isIgnite && canApplyIgnite)
        {
            isIgnite = _isIgnite;
            ignitedTimer = 4;
            fx.IgniteFxFor(4);
        }

        if (_isChilled && canApplyChill)
        {
            isChilled = _isChilled;
            chilledTimer = 2;

            float slowPercentage = 0.2f;
            GetComponent<Entity>().SLowEntityBy(slowPercentage, 2);
            fx.ChillFxFor(2);
        }

        if (_isShocked && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_isShocked);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                ThunderShockStrikeLogic();
            }
        }

        isIgnite = _isIgnite;
        isChilled = _isChilled;
        isShocked = _isShocked;
    }

    public void ApplyShock(bool _isShocked)
    {
        if (isShocked)
            return;

        isShocked = _isShocked;
        shockedTimer = 2;
        fx.ShockFxFor(2);
    }

    private void ThunderShockStrikeLogic()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;               //Take the closest enemy position
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefabs, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockThunderController>().SetupThunder(shockDamage, closestEnemy.GetComponent<CharacterStat>());
        }
    }
    private void ApplyIgniteDamage()
    {
        DecreaseHealthBy(igniteDamage);
        if (currentHP < 0 && !isDead)
            Die();

        igniteDamageTimer = igniteDamageCoolDown;
    }
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    #endregion

    #region Stats Calculate
    protected int TargetArmors(CharacterStat _targetStat, int totalDamage)
    {
        if (isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStat.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStat.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool CanAvoidAttack(CharacterStat _targetStat)
    {
        int totalEvasion = _targetStat.evasion.GetValue() + _targetStat.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)            //ATTACK MISSED
        {
            _targetStat.OnEvasion();
            return true;
        }

        return false;
    }

    public bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCritChance)
        {
            return true;
        }

        return false;
    }

    public int CritPowerDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.1f;             //Crit chance as %

        float totalCritDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(totalCritDamage);
    }


    public int GetMaxHealthValue()
    {
        return maxHP.GetValue() + vitality.GetValue() * 5;          //1 vitality point = 5 health
    }
    #endregion

    protected virtual void Die() => isDead = true;

    public void killEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    public Stats GetStats(StatType _statType)
    {
        if (_statType == StatType.strength)
            return strength;
        else if (_statType == StatType.intelligence)
            return intelligence;
        else if (_statType == StatType.agility)
            return agility;
        else if (_statType == StatType.vitality)
            return vitality;
        else if (_statType == StatType.damage)
            return damage;
        else if (_statType == StatType.critChance)
            return critChance;
        else if (_statType == StatType.critPower)
            return critPower;
        else if (_statType == StatType.armor)
            return armor;
        else if (_statType == StatType.health)
            return maxHP;
        else if (_statType == StatType.evasion)
            return evasion;
        else if (_statType == StatType.MagicResistant)
            return MagicResistant;
        else if (_statType == StatType.fireDamage)
            return fireDamage;
        else if (_statType == StatType.iceDamage)
            return iceDamage;
        else if (_statType == StatType.lightingDamage)
            return lightingDamage;
        else
            return null;
    }
}
