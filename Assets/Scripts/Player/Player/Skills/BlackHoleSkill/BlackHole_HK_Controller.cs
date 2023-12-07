using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHole_HK_Controller : MonoBehaviour
{
    private SpriteRenderer sr;

    private KeyCode myHotKey;
    private TextMeshProUGUI myText;
    private Transform enemiesTransform;
    private BlackHoleSkillController blackholeSkill;

    public void SetupHotKey(KeyCode _hotKey, Transform _myEnemy, BlackHoleSkillController _BLskill)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        enemiesTransform = _myEnemy;
        blackholeSkill = _BLskill;
        myHotKey = _hotKey;
        myText.text = myHotKey.ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(myHotKey))
        {
            blackholeSkill.AddEnemyToList(enemiesTransform);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
