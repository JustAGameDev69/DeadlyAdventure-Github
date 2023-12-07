using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Skill Setting")]
    [SerializeField] private SkillTreeSlot swordSkillUnlockButton;
    public bool swordSkillUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefabs;
    [SerializeField] private Vector2 lauchForce;
    [SerializeField] private float swordGravity;

    private Vector2 finalDir;

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefabs;
    [SerializeField] private Transform dotParent;

    [Header("Bounce Sword")]
    [SerializeField] private SkillTreeSlot bounceSwordUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Pierce Sword")]
    [SerializeField] private SkillTreeSlot pierceSwordUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin Sword")]
    [SerializeField] private SkillTreeSlot spinSwordUnlockButton;
    [SerializeField] private float hitCooldown;
    [SerializeField] private float spinDuration;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinGravity;

    [Header("Passive Skill")]
    [SerializeField] private SkillTreeSlot timeSwordUnlockButton;
    public bool timeSwordUnlocked { get; private set; }
    [SerializeField] private SkillTreeSlot timeSwordUpgradeUnlockButton;
    public bool timeSwordUpgradeUnlocked { get; private set; }

    [SerializeField]private float freezeDuration;

    private GameObject[] dots;

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * lauchForce.x, AimDirection().normalized.y * lauchForce.y);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();

        swordSkillUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        timeSwordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeSword);
        timeSwordUpgradeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeSwordUpgrade);
        bounceSwordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        pierceSwordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        spinSwordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);

    }

    #region Unlock Sword Skill

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
        UnlockTimeSword();
        UnlockTimeSwordUpgrade();
    }

    private void UnlockTimeSword()
    {
        if (timeSwordUnlockButton.unlocked)
            timeSwordUnlocked = true;
    }

    private void UnlockTimeSwordUpgrade()
    {
        if(timeSwordUpgradeUnlockButton.unlocked)
            timeSwordUpgradeUnlocked = true;
    }

    private void UnlockSword()
    {
        if (swordSkillUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordSkillUnlocked = true;
        }
    }

    private void UnlockBounceSword()
    {
        if(bounceSwordUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if (pierceSwordUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinSwordUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }

    #endregion

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if(swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefabs, player.transform.position, transform.rotation);
        SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

        if(swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount);
        }
        else if(swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }
        else if(swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeDuration);
        AudioManager.instance.PlaySFX(27, player.transform);
        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim Direction
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefabs, player.transform.position, Quaternion.identity, dotParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float _t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(AimDirection().normalized.x * lauchForce.x, AimDirection().normalized.y * lauchForce.y) * _t + 0.5f * (Physics2D.gravity * swordGravity) * (_t * _t);

        return position;
    }
    #endregion
}
