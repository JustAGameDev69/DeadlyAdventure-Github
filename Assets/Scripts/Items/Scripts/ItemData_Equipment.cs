using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique Effect")]
    public float itemCoolDown;
    public ItemEffect[] itemEffect;
    [TextArea]
    public string itemEffectDescription;

    [Header("Major Stats")]
    public int strength;
    public int intelligence;
    public int agility;
    public int vitality;

    [Header("Offensice Stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive Stats")]
    public int armor;
    public int evasion;
    public int MagicResistant;

    [Header("Magic Stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    private int descriptionLength;

    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffect)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifiers(strength);
        playerStats.agility.AddModifiers(agility);
        playerStats.intelligence.AddModifiers(intelligence);
        playerStats.vitality.AddModifiers(vitality);

        playerStats.damage.AddModifiers(damage);
        playerStats.critChance.AddModifiers(critChance);
        playerStats.critPower.AddModifiers(critPower);

        playerStats.armor.AddModifiers(armor);
        playerStats.evasion.AddModifiers(evasion);
        playerStats.MagicResistant.AddModifiers(MagicResistant);

        playerStats.fireDamage.AddModifiers(fireDamage);
        playerStats.iceDamage.AddModifiers(iceDamage);
        playerStats.lightingDamage.AddModifiers(lightingDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifiers(strength);
        playerStats.agility.RemoveModifiers(agility);
        playerStats.intelligence.RemoveModifiers(intelligence);
        playerStats.vitality.RemoveModifiers(vitality);

        playerStats.damage.RemoveModifiers(damage);
        playerStats.critChance.RemoveModifiers(critChance);
        playerStats.critPower.RemoveModifiers(critPower);

        playerStats.armor.RemoveModifiers(armor);
        playerStats.evasion.RemoveModifiers(evasion);
        playerStats.MagicResistant.RemoveModifiers(MagicResistant);

        playerStats.fireDamage.RemoveModifiers(fireDamage);
        playerStats.iceDamage.RemoveModifiers(iceDamage);
        playerStats.lightingDamage.RemoveModifiers(lightingDamage);
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "Strength");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(agility, "Agility");
        AddItemDescription(vitality, "Vitality");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "CritChance");
        AddItemDescription(critPower, "CritPower");

        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(MagicResistant, "Magic Resistant");

        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightingDamage, "Lighting Damage");

        if (descriptionLength < 4)
        {
            for (int i = 0; i < 5 - descriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        if (itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }

        return sb.ToString();
    }

    private void AddItemDescription(int _value, string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            if (_value > 0)
            {
                sb.Append("+ " + _value + " " + _name);
            }

            descriptionLength++;
        }
    }

}
