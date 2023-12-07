using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected Player player;

    [Header("PopUp Text")]
    [SerializeField] private GameObject popUpTextPrefabs;

    [Header("Flash Effect")]
    [SerializeField] private Material hitMaterial;
    public float flashDuration;
    private Material originMaterial;

    protected SpriteRenderer sr;

    [SerializeField] private GameObject myHealthBar;


    [Header("Aliment Effect")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Aliment particles")]
    [SerializeField] private ParticleSystem igniteFX;
    [SerializeField] private ParticleSystem chillFX;
    [SerializeField] private ParticleSystem shockFX;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalHitFX;

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;
        originMaterial = sr.material;
    }

    public void CreatePopupText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1, 2);

        Vector3 positionOffset = new Vector3(randomX, randomY);
        GameObject newText = Instantiate(popUpTextPrefabs, transform.position + positionOffset, Quaternion.identity);
        newText.GetComponent<TextMeshPro>().text = _text;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMaterial;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originMaterial;
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            myHealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            myHealthBar.SetActive(true);
            sr.color = Color.white;
        }
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void IgniteFxFor(float _second)
    {
        igniteFX.Play();

        InvokeRepeating("IgniteColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }

    public void ShockFxFor(float _second)
    {
        shockFX.Play();
        InvokeRepeating("ShockedColorFX", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }

    public void ChillFxFor(float _second)
    {
        chillFX.Play();
        InvokeRepeating("ChillColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _second);
    }

    private void IgniteColorFX()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ShockedColorFX()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    public void CreateHitFx(Transform _target, bool _critical)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-0.1f, 0.1f);
        float yPosition = Random.Range(-0.1f, 0.1f);

        Vector3 hitFXRotation = new Vector3(0, 0, zRotation);

        GameObject hitPrefabs = hitFx;
        if (_critical)
        { 
            hitPrefabs = criticalHitFX;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFXRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFx = Instantiate(hitPrefabs, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity, _target);

        newHitFx.transform.Rotate(hitFXRotation);

        Destroy(newHitFx, 0.5f);
    }

}
