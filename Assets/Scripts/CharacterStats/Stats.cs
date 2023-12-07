using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Stats
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefaulValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifiers(int _modifiers)
    {
        modifiers.Add(_modifiers);
    }

    public void RemoveModifiers(int _modifiers)
    {
        modifiers.Remove(_modifiers);
    }
}
