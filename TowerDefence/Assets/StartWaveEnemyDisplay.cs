using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartWaveEnemyDisplay : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text numOfEnemy; 

    private void OnValidate()
    {
        TryGetComponent(out img);
        numOfEnemy = GetComponentInChildren<TMP_Text>();
    }
    public void SetSprite(Sprite spr)
    {
        img.overrideSprite = spr;
    }
    public void SetText(string str)
    {
        numOfEnemy.text ="X " + str;
    }
}
