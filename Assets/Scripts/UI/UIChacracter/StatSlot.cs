using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
            statNameText.text = statName;
    }

    private void Start()
    {
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStats(statType).GetValue().ToString();

            if (statType == StatType.health)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();
            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();
            if (statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();
            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
            if (statType == StatType.MagicResistant)
                statValueText.text = (playerStats.MagicResistant.GetValue() + (playerStats.intelligence.GetValue()) * 3).ToString();


        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        Vector2 mousePosition = Input.mousePosition;

        float xOffset;
        float yOffset;

        if (mousePosition.x < 460)
            xOffset = 260;
        else
            xOffset = -260;

        if (mousePosition.y < 220)
            yOffset = 200;
        else
            yOffset = 100;

        ui.statsToolTip.transform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
        ui.statsToolTip.ShowStatToolTip(statDescription);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statsToolTip.HideStatToolTip();
    }
}
