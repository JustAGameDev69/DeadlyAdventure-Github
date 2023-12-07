using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour, ISaveManager
{
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject inGameUI;

    public SkillToolTip skillToolTip;
    public ItemToolTip itemToolTip;
    public StatsToolTip statsToolTip;
    public CraftWindow craftWindow;

    [SerializeField] private VolumeSlider[] volumeSettings;

    private void Awake()
    {
        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
            SwitchWithKeyTo(characterUI);
        if (Input.GetKeyUp(KeyCode.B))
            SwitchWithKeyTo(craftUI);
        if (Input.GetKeyUp(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);
        if (Input.GetKeyUp(KeyCode.O))
            SwitchWithKeyTo(optionUI);
    }

    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;

            if(!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if(_menu != null)
        {
            AudioManager.instance.PlaySFX(7, null);
            _menu.SetActive(true);
        }

        if(GameManager.instance != null)
        {
            if (_menu == inGameUI)                          //this is inventory are turn on
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.25f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach(VolumeSlider item in volumeSettings)
            {
                if(item.parameter == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach(VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }
}
