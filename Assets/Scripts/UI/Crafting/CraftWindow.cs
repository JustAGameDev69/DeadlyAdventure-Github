using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image[] materialImage;
    [SerializeField] private Button craftButton;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)                  //Cleaning the material
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for(int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            materialImage[i].sprite = _data.craftingMaterials[i].data.itemIcon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.color = Color.white;
            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();

        }

        itemIcon.sprite = _data.itemIcon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }
}
