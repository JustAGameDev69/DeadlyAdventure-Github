using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();
    private RectTransform myTransform;
    private Slider slider;
    private CharacterStat characterStat => GetComponentInParent<CharacterStat>();

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = characterStat.GetMaxHealthValue();
        slider.value = characterStat.currentHP;
    }

    private void OnEnable()
    {
        entity.onFLipped -= FlipUI;
        characterStat.onHealthChange -= UpdateHealthUI;
    }

    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }

    private void OnDisable()
    {
        if (entity != null)
            entity.onFLipped += FlipUI;

        if (characterStat != null)
            characterStat.onHealthChange += UpdateHealthUI;
    }
}
