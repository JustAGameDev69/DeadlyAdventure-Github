using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player drop")]
    [SerializeField] private float chanceToLooseItem;
    [SerializeField] private float chanceToLooseMaterial;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialToLoose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceToLooseItem)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }


        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterial)
            {
                DropItem(item.data);
                materialToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialToLoose.Count; i++)
        {
            inventory.RemoveItems(materialToLoose[i].data);
        }

    }
}
