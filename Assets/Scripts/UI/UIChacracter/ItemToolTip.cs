using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : UI_ToolTip
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemTypeText;
    [SerializeField] private Text itemDescriptiom;
    [SerializeField] private int defaultFontSize = 38;

    public void ShowToolTip(ItemData_Equipment _itemData)
    {
        if (_itemData == null)
            return;

        itemNameText.text = _itemData.itemName;
        itemTypeText.text = _itemData.equipmentType.ToString();
        itemDescriptiom.text = _itemData.GetDescription();

        if (itemNameText.text.Length > 19)
            itemNameText.fontSize = Mathf.RoundToInt(itemNameText.fontSize * .7f);
        else
            itemNameText.fontSize = defaultFontSize;

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        itemNameText.fontSize = defaultFontSize;
        gameObject.SetActive(false);
    }
}
