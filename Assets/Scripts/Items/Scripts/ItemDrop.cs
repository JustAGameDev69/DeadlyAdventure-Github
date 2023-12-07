using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;

    [SerializeField] private GameObject dropPrefabs;

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (possibleDrop.Length <= 0)
                return;

            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }



        for (int i = 0; i < possibleItemDrop; i++)
        {
            if (dropList.Count > 0)
            {
                ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];

                dropList.Remove(randomItem);
                DropItem(randomItem);

            }
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        GameObject newDrop = Instantiate(dropPrefabs, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(12, 15));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
