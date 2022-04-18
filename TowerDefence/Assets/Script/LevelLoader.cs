using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject lv;
    private Dictionary<string, int> levelOpened;
    private LevelUnlocked lvl;
    private List<string> turret;//wait
    void Start()
    {
        //lvl = SaveSystem.LoadLevel(); //= new LevelUnlocked();
        //levelOpened = lvl.GetLevelList();
        //SaveTesting();
        //LevelDisplayer();
    }

    public void SaveTesting()
    {
        Dictionary<string, int> ok=new Dictionary<string, int>();
        ok.Add("TutorialLevel", 0);
        ok.Add("Sector 78", 1);
        lvl.SetLevel(ok);
        SaveSystem.SaveLevelProgression(lvl);
    }
    public void SaveProgress()
    {
        SaveSystem.SaveLevelProgression(lvl);
    }
    public void LevelDisplayer()
    {
        for (int i = 0; i < lv.transform.childCount; i++)
        {
            if (levelOpened.ContainsKey(lv.transform.GetChild(i).name))
            {
                Debug.Log("Yes we do have "+ lv.transform.GetChild(i).name + " at " + i);
                lv.transform.GetChild(levelOpened[lv.transform.GetChild(i).name]).GetComponent<GlowingButton>().OpenLevelPath();
                lv.transform.GetChild(levelOpened[lv.transform.GetChild(i).name]).gameObject.SetActive(true);
            }
                
        }
    }
}
