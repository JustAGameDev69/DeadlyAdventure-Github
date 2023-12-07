using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : UI_ToolTip
{
    [SerializeField] private Text skillDescription;
    [SerializeField] private Text skillName;
    [SerializeField] private Text skillCost;

    public void ShowToolTip(string _skillDescription, string _skillName, int _price)
    {
        skillDescription.text = _skillDescription;
        skillName.text = _skillName;
        skillCost.text = "Cost : " + _price;
        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
