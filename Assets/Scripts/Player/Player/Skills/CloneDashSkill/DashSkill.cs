using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{

    [Header("Dash")]
    [SerializeField] private SkillTreeSlot dashUnlockedButton;
    public bool dashUnlocked { get; private set; }

    [Header("Clone on dash")]
    [SerializeField] private SkillTreeSlot cloneOnDashUnlockedButton;
    public bool cloneOnDashUnlocked { get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private SkillTreeSlot cloneOnArrivalUnlockedButton;
    public bool cloneOnArrivalUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

    }

    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();
    }

    protected override void Start()
    {
        base.Start();

        dashUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }

    private void UnlockDash()
    {
        if(dashUnlockedButton.unlocked)
            dashUnlocked = true;
    }

    private void UnlockCloneOnDash()
    {
        if(cloneOnDashUnlockedButton.unlocked)
            cloneOnDashUnlocked = true;
    }

    private void UnlockCloneOnArrival()
    {
        if(cloneOnArrivalUnlockedButton.unlocked)
            cloneOnArrivalUnlocked = true;
    }

    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }

    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
    }
}
