using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefabs;
    [SerializeField] private float crystalDuration;

    [Header("Crystal Spawn")]
    [SerializeField] private SkillTreeSlot crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explode")]
    [SerializeField] private SkillTreeSlot crystalExplodeButton;
    public bool crystalExplodeUnlocked;

    [Header("Crystal Follow")]
    [SerializeField] private SkillTreeSlot crystalFollowUnlockButton;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool crystalFollowUnlocked;

    [Header("Multiple crystal")]
    [SerializeField] private SkillTreeSlot multipleCrystalUnlockButton;
    [SerializeField] private int crystalAmount;
    [SerializeField] private float multiCrystalCooldown;
    [SerializeField] private bool multipleCrystalUnlocked;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalRemain = new List<GameObject>();

    [Header("Crystal Clone Spawn")]
    [SerializeField] private SkillTreeSlot cloneSpawnButton;
    [SerializeField] private bool cloneSpawnUnlocked;

    private GameObject currentCrystal;

    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        crystalExplodeButton.GetComponent<Button>().onClick.AddListener(UnlockExplodeCrystal);
        cloneSpawnButton.GetComponent<Button>().onClick.AddListener(UnlockCloneSpawn);
        crystalFollowUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFollowCrystal);
        multipleCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleCrystal);
    }

    #region Unlock Skill Function

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockExplodeCrystal();
        UnlockFollowCrystal();
        UnlockMultipleCrystal();
        UnlockCloneSpawn();
    }

    private void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            crystalUnlocked = true;
    }
    private void UnlockFollowCrystal()
    {
        if (crystalFollowUnlockButton.unlocked)
            crystalFollowUnlocked = true;
    }

    private void UnlockExplodeCrystal()
    {
        if (crystalExplodeButton.unlocked)
            crystalExplodeUnlocked = true;
    }

    private void UnlockMultipleCrystal()
    {
        if (multipleCrystalUnlockButton.unlocked)
            multipleCrystalUnlocked = true;
    }
    private void UnlockCloneSpawn()
    {
        if (cloneSpawnButton.unlocked)
            cloneSpawnUnlocked = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystalSkill())             //Crystal follow upgrade
            return;

        //Base crystal follow skill
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (crystalFollowUnlocked)
                return;

            //Teleport player to the crystal position
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneSpawnUnlocked)                   //Clone instead of crystal is an upgrade of crystal skills which disabled other crystal skill!
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
            currentCrystal.GetComponent<CrystalSkillController>()?.CrystalExplode();
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefabs, player.transform.position, Quaternion.identity);
        CrystalSkillController currentCrystalScript = currentCrystal.GetComponent<CrystalSkillController>();

        currentCrystalScript.SetupCrystal(crystalDuration, crystalExplodeUnlocked, crystalFollowUnlocked, moveSpeed, FindClosetEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<CrystalSkillController>().ChooseRandomEnemies();

    private bool CanUseMultiCrystalSkill()
    {
        if (multipleCrystalUnlocked)
        {
            if (crystalRemain.Count > 0)
            {
                if (crystalRemain.Count == crystalAmount)                   //Skill reset after time if player didn't use all of it;
                    Invoke("ResetMultiCrystalAbility", useTimeWindow);

                skillCooldown = 0;
                GameObject crystalSpawn = crystalRemain[crystalRemain.Count - 1];
                GameObject newCrystal = Instantiate(crystalSpawn, player.transform.position, Quaternion.identity);

                crystalRemain.Remove(crystalSpawn);

                newCrystal.GetComponent<CrystalSkillController>().
                    SetupCrystal(crystalDuration, crystalExplodeUnlocked, crystalFollowUnlocked, moveSpeed, FindClosetEnemy(newCrystal.transform), player);

                if (crystalRemain.Count <= 0)
                {
                    skillCooldown = multiCrystalCooldown;
                    RefillCrystal();
                }

                return true;
            }

        }

        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = crystalAmount - crystalRemain.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalRemain.Add(crystalPrefabs);
        }
    }

    private void ResetMultiCrystalAbility()                     //Fix player not use all of 3 crystal in an amount of times;
    {
        if (cooldownTimer > 0)                                  //If already in cooldown, no longer had to reset this ability;
            return;

        cooldownTimer = multiCrystalCooldown;
        RefillCrystal();
    }
}
