using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MoneyInGameUI : MonoBehaviour
{
    public bool isTmp = true;

    public Text Money;
    public TMP_Text money_Text;

    void Update()
    {
        if (!isTmp)
        {
            Money.text =PlayerStat.moneyInGame.ToString() + " $";
            return;
        }
        money_Text.text = PlayerStat.moneyInGame.ToString() + " $";
    }
}
