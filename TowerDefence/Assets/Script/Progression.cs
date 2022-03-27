using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progression : MonoBehaviour
{
    public static int money;
    public static Data saveData;
    private void Awake()
    {
        LoadsaveData();
        SetMoney();
    }
    public static void LoadsaveData()
    {
        saveData = SaveSystem.LoadData();
    } 
    public static void SetMoney()
    {
        money = saveData.GetMoney();
    }
    public static void SaveData()
    {
        SaveSystem.Savedata(saveData);
    }
}
