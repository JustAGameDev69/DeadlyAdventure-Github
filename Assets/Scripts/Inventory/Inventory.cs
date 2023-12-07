using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItem;
    public List<InventoryItem> equipment;
    public List<InventoryItem> inventory;
    public List<InventoryItem> stash;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSLotParent;
    [SerializeField] private Transform statSlotParent;

    [Header("item Cooldown")]
    private float lastTimeUseFlask;
    private float lastTimeUseArmorBuff;

    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    public float flaskCoolDown { get; private set; }
    private float armorCoolDown;

    private ItemSlot[] inventoryItemSlots;
    private ItemSlot[] stashItemSlots;
    private EquipmentSlot[] equipmentSlot;
    private StatSlot[] statSlot;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<ItemSlot>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<ItemSlot>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        equipmentSlot = equipmentSLotParent.GetComponentsInChildren<EquipmentSlot>();

        statSlot = statSlotParent.GetComponentsInChildren<StatSlot>();

        AddStartingItem();
    }

    private void AddStartingItem()
    {

        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }

            return;
        }

        for (int i = 0; i < startingItem.Count; i++)
        {
            if (startingItem[i] != null)
                AddItem(startingItem[i]);
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;              //covert from ItemData to ItemData_Equipment
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment OldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                OldEquipment = item.Key;
        }

        if (OldEquipment != null)
        {
            UnequipItem(OldEquipment);
            AddItem(OldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItems(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlots.Length)
        {
            return false;
        }

        return true;
    }

    public void AddItem(ItemData _itemData)
    {
        if (_itemData.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_itemData);
        else if (_itemData.itemType == ItemType.Material)
            AddToStash(_itemData);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _itemData)
    {
        if (stashDictionary.TryGetValue(_itemData, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_itemData);
            stash.Add(newItem);
            stashDictionary.Add(_itemData, newItem);
        }
    }

    private void AddToInventory(ItemData _itemData)
    {
        if (inventoryDictionary.TryGetValue(_itemData, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_itemData);
            inventory.Add(newItem);
            inventoryDictionary.Add(_itemData, newItem);
        }
    }

    public void RemoveItems(ItemData _itemData)
    {
        if (inventoryDictionary.TryGetValue(_itemData, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_itemData);
            }
            else
                value.RemoveStack();
        }


        if (stashDictionary.TryGetValue(_itemData, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_itemData);
            }
            else
                stashValue.RemoveStack();
        }


        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requirementMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requirementMaterials.Count; i++)
        {

            if (stashDictionary.TryGetValue(_requirementMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requirementMaterials[i].stackSize)
                {
                    Debug.Log("Not enough Materials");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("Not enough Materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItems(materialsToRemove[i].data);
        }

        AddItem(_itemToCraft);
        Debug.Log("Item crafted" + _itemToCraft.name);
        return true;

    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        bool canUseFlask = Time.time > lastTimeUseFlask + flaskCoolDown;

        if (currentFlask == null)
            return;

        if (canUseFlask)
        {
            flaskCoolDown = currentFlask.itemCoolDown;
            currentFlask.Effect(null);
            lastTimeUseFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown");

    }

    public bool CanUseArmorEffect()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUseArmorBuff + armorCoolDown)
        {
            armorCoolDown = currentArmor.itemCoolDown;
            lastTimeUseArmorBuff = Time.time;
            return true;
        }

        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)
        {

            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }

        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }

    }

#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Scripts/Items/MainItems" });

        foreach (string SOname in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOname);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif

}
