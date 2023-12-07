using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("After Image Effect")]
    [SerializeField] private GameObject afterImgPrefabs;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImgCooldown;
    private float afterImgCooldownTimer;

    [Header("Screen Shake FX")]
    [SerializeField] private float shakeMultiplier;
    private CinemachineImpulseSource screenShake;
    public Vector3 shakeSwordPower;
    public Vector3 shakeHighDamage;

    [Space]
    [SerializeField] private ParticleSystem dustFX;


    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        afterImgCooldownTimer -= Time.deltaTime;
    }

    public void CreateAfterImage()
    {
        if (afterImgCooldownTimer < 0)
        {
            afterImgCooldownTimer = afterImgCooldown;
            GameObject newAfterImage = Instantiate(afterImgPrefabs, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }
    }

    public void ScreenShake(Vector3 _direction)
    {
        screenShake.m_DefaultVelocity = new Vector3(_direction.x * player.facingDir, _direction.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void PlayDustFX()
    {
        if (dustFX != null)
            dustFX.Play();
    }

}
