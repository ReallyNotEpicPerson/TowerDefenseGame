using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private Text UI;

    void Update()
    {
        UI.text ="\tMoney : " + Progression.money.ToString();
    }
}
