using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHoleSkill : Skill
{
    [SerializeField] private SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked;
    [SerializeField] private GameObject blackHolePrefabs;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private int attackAmount;
    [SerializeField] private float cloneAttackCooldown;
    [SerializeField] private float blackholeDuration;

    BlackHoleSkillController currentBlackHole;

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockBalckHole();
    }

    private void UnlockBalckHole()
    {
        if (blackHoleUnlockButton.unlocked)
            blackHoleUnlocked = true;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefabs, player.transform.position, Quaternion.identity);
        currentBlackHole = newBlackHole.GetComponent<BlackHoleSkillController>();
        currentBlackHole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, attackAmount , cloneAttackCooldown, blackholeDuration);
        AudioManager.instance.PlaySFX(3, player.transform);
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    protected override void Start()
    {
        base.Start();

        blackHoleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBalckHole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillFinished()
    {
        if(!currentBlackHole)
            return false;

        if (currentBlackHole.canPlayerExitState)
        {
            currentBlackHole = null;
            return true;
        }

        return false;
    }

    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }
}
