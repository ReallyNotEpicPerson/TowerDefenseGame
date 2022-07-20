using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "FMS")]
public class Setting : ScriptableObject
{
    public int[] characterLineUp;
    [SerializeField] private int moneyEachMatch;
    public string NextLevel;
    //public int[] LevelOpened;
    public int currentLevelIndex;
    public bool OpenManualAtStart = false;

    public void SetRewardMoney(int m)
    {
        moneyEachMatch = m;
    }
    public void SetLineUp(int[] l)
    {
        characterLineUp = l;
        //SetDirty();
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
    public int GetMoney()
    {
        return moneyEachMatch;
    }
}
