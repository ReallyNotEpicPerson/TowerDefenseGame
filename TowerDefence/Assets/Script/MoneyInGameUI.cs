using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MoneyInGameUI : MonoBehaviour
{
    public bool isTmp = true;
    public Vector3 moneyGainedPosition;
    public static Vector3 pos;
    public Text Money;
    public TMP_Text money_Text;

    public void OnValidate()
    {
        if (money_Text != null)
        {
            isTmp = true;
        }
        else
        {
            isTmp = false;
        }
    }
    private void Awake()
    {
        pos = moneyGainedPosition;
        pos.z = 0;
    }
    void Update()
    {
        if (isTmp)
        {        
            money_Text.text = PlayerStat.moneyInGame.ToString();
            return;
        } 
        Money.text =PlayerStat.moneyInGame.ToString() ;
       
    }
}
