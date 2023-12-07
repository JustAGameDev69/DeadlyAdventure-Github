using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("Lightning Reflexes")]
    [SerializeField] private SkillTreeSlot lightingReflexesUnlockButton;
    public bool lightingReflexesUnlocked;

    [Header("Mirror Mirage")]
    [SerializeField] private SkillTreeSlot mirrorMirageUnlockButton;
    public bool mirrorMirageUnlocked;

    protected override void Start()
    {
        base.Start();

        lightingReflexesUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockLightningReflexes);
        mirrorMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirrorMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockLightningReflexes();
        UnlockMirrorMirage();
    }

    private void UnlockLightningReflexes()
    {
        if (lightingReflexesUnlockButton.unlocked && !lightingReflexesUnlocked)
        {
            player.stats.evasion.AddModifiers(10);
            Inventory.instance.UpdateStatUI();
            lightingReflexesUnlocked = true;
        }
    }

    private void UnlockMirrorMirage()
    {
        if(mirrorMirageUnlockButton.unlocked)
            mirrorMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (mirrorMirageUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector3(2 * player.facingDir, 0));
    }

}
