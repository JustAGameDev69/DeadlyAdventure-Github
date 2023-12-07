using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlot : ItemSlot
{

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        item.data = _data;

        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;

        if (itemText.text.Length > 10)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 24;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
