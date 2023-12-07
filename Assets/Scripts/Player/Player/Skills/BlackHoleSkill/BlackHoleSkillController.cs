using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefabs;
    [SerializeField] private List<KeyCode> keyCodeList;

    private int attackAmount;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;

    private bool canCreateHotKey = true;        //Fix enemy walk inside after finish set hotkeys
    private bool cloneAttack;
    private bool canGrow = true;
    private bool canShrink;
    private float growSpeed;
    private float shrinkSpeed;
    private float maxGrowSize;
    private float blackholeDuration;
    private bool playerCanDissapear = true;            //Fix player could dissapear when press R after using this skill

    private List<GameObject> createHotKeys = new List<GameObject>();
    private List<Transform> targets = new List<Transform>();

    public bool canPlayerExitState {  get; private set; }
    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _attackAmount, float _cloneAttackCoolDown, float _blackholeDuration)
    {
        maxGrowSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        attackAmount = _attackAmount;
        cloneAttackCooldown = _cloneAttackCoolDown;
        blackholeDuration = _blackholeDuration;

        if (SkillManager.instance.clone.crystalInsteadofClone)
            playerCanDissapear = false;
    }

    private void Update()
    {

        cloneAttackTimer -= Time.deltaTime;
        blackholeDuration -= Time.deltaTime;

        if(blackholeDuration < 0)
        {
            blackholeDuration = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleSkill();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxGrowSize, maxGrowSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        cloneAttack = true;
        DestroyHotKey();
        canCreateHotKey = false;

        if (playerCanDissapear)
        {
            playerCanDissapear = false;
            PlayerManager.instance.player.fx.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttack && attackAmount > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if (Random.Range(0, 100) > 50)      //50% the clone miss the attack
                xOffset = 1.5f;
            else
                xOffset = -1.5f;

            if(SkillManager.instance.clone.crystalInsteadofClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }


            attackAmount--;
            if (attackAmount <= 0)
            {
                Invoke("FinishBlackHoleSkill", 1f);
            }
        }
    }

    private void FinishBlackHoleSkill()
    {
        DestroyHotKey();
        canPlayerExitState = true;
        canShrink = true;
        cloneAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()  != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

            //targets.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent <Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count < 0)
            return;

        if (!canCreateHotKey)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefabs, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKeys.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHole_HK_Controller newHotkeyScript = newHotKey.GetComponent<BlackHole_HK_Controller>();

        newHotkeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

    private void DestroyHotKey()
    {
        if (createHotKeys.Count <= 0)
            return;

        for (int i = 0; i < createHotKeys.Count; i++)
        {
            Destroy(createHotKeys[i]);
        }
    }
}
