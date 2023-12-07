using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("Parry")]
    [SerializeField] private SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked { get; private set; }

    [Header("Parry Upgrade")]
    [SerializeField] private SkillTreeSlot parryUpgradeUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float healingPercentage;
    public bool parryUpgradeUnlocked { get; private set; }

    [Header("Parry Of Perfection")]
    [SerializeField] private SkillTreeSlot parryOfPerfectionUnlockButton;
    public bool parryOfPerfectionUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (parryUpgradeUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValue() * healingPercentage);
            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        parryUpgradeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryUpgrade);
        parryOfPerfectionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryOfPerfection);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryUpgrade();
        UnlockParryOfPerfection();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
    }
    private void UnlockParryUpgrade()
    {
        if (parryUpgradeUnlockButton.unlocked)
            parryUpgradeUnlocked = true;
    }

    private void UnlockParryOfPerfection()
    {
        if (parryOfPerfectionUnlockButton.unlocked)
            parryOfPerfectionUnlocked = true;
    }

    public void MakeCloneOnParry(Transform _respawnTransform)
    {
        if (parryOfPerfectionUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }
}
