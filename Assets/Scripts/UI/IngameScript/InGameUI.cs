using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image flaskImage;

    [Header("Souls infor")]
    [SerializeField] private Text currentSoul;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    private SkillManager skills;

    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChange += UpdateHealthUI;

        skills = SkillManager.instance;
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHP;
    }

    void Update()
    {
        UpdateSoulUI();

        if (Input.GetKeyDown(KeyCode.E) && skills.dash.dashUnlocked)
            SetCoolDownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCoolDownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCoolDownOf(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.throwSword.swordSkillUnlocked)
            SetCoolDownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackHole.blackHoleUnlocked)
            SetCoolDownOf(blackHoleImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCoolDownOf(flaskImage);

        CheckCoolDownOf(dashImage, skills.dash.skillCooldown);
        CheckCoolDownOf(parryImage, skills.parry.skillCooldown);
        CheckCoolDownOf(crystalImage, skills.crystal.skillCooldown);
        CheckCoolDownOf(swordImage, skills.throwSword.skillCooldown);
        CheckCoolDownOf(blackHoleImage, skills.blackHole.skillCooldown);
        CheckCoolDownOf(flaskImage, Inventory.instance.flaskCoolDown);
    }

    private void UpdateSoulUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.GetCurrency();

        currentSoul.text = ((int)soulsAmount).ToString();
    }

    private void SetCoolDownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCoolDownOf(Image _image, float _coolDown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _coolDown * Time.deltaTime;
    }

    public void OnClickExitButton() => SceneManager.LoadScene(0);
}
