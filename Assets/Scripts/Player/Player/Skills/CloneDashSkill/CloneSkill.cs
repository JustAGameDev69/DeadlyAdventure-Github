using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [Header("Split Skill")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefabs;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Split Attack")]
    [SerializeField] private SkillTreeSlot splitAttackUnlockButton;
    [SerializeField] private float splitAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Split Attack Upgrade")]
    [SerializeField] private SkillTreeSlot splitSkillUpgradeUnlockButton;
    [SerializeField] private float splitSkillUpgradeMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multiple Clone")]
    [SerializeField] private SkillTreeSlot multipleCloneUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal InsteadOf Clone")]
    [SerializeField] private SkillTreeSlot crystalInsteadOfCloneUnlockButton;
    public bool crystalInsteadofClone;

    protected override void Start()
    {
        base.Start();

        splitAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSplitAttack);
        splitSkillUpgradeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSplitSkillUpgrade);
        multipleCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleClone);
        crystalInsteadOfCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInsteadOfClone);
    }

    #region Unlock SKill

    protected override void CheckUnlock()
    {
        UnlockCrystalInsteadOfClone();
        UnlockMultipleClone();
        UnlockSplitAttack();
        UnlockSplitSkillUpgrade();
    }

    private void UnlockSplitAttack()
    {
        if (splitAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = splitAttackMultiplier;
        }
    }

    private void UnlockSplitSkillUpgrade()
    {
        if (splitSkillUpgradeUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = splitSkillUpgradeMultiplier;
        }
    }

    private void UnlockMultipleClone()
    {
        if(multipleCloneUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInsteadOfClone()
    {
        if(crystalInsteadOfCloneUnlockButton.unlocked)
        {
            crystalInsteadofClone = true;
        }
    }

    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offSet)
    {

        if (crystalInsteadofClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefabs);

        newClone.GetComponent<CloneSkillController>().SetupClone(_clonePosition, cloneDuration, canAttack, _offSet, FindClosetEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player, attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3(1.5f * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offSet)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(_transform, _offSet);
    }
}
