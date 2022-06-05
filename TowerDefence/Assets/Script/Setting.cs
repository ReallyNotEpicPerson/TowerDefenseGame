using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "FMS")]
public class Setting : ScriptableObject
{
    public int[] characterLineUp;
    [SerializeField] private int moneyEachMatch;

    public int[] LevelOpened;
    //SaveSystem save

    public void SetRewardMoney(int m)
    {
        moneyEachMatch = m;
    }
    public void SetLineUp(int[] l)
    {
        characterLineUp = l;
        EditorUtility.SetDirty(this);
    } 
    public int GetMoney()
    {
        return moneyEachMatch;
    }
}
