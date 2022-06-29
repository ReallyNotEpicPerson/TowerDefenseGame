using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public Setting Setting;
    public List<Transform> level;
    private Dictionary<string, Transform> levelDict = new Dictionary<string, Transform>();
    public List<Transform> levelStarList;
    private LevelProgession levelProgession;
    /*private void OnValidate()
    {
        level.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            level.Add( transform.GetChild(i) );
        }
    }
    public List<TMP_Text> levelName;
    private void OnValidate()
    {
        levelName.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {            
            if(transform.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text ok))
                levelName.Add(ok);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).name = levelName[i].text;
        }
    }*/
    void Awake()
    {
        for (int i = 0; i < level.Count; i++)
        {
            levelDict.Add(level[i].name, level[i]);
        }
        levelProgession = SaveSystem.LoadLevelProgression();
        for (int i = 0; i < level.Count; i++)
        {
            if (i < levelProgession.level.Count)
            {
                levelDict[levelProgession.level[i]].gameObject.SetActive(true);
            }
            else
            {
                level[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < level.Count; i++)
        {
            switch (levelProgession.star[i])
            {
                case 0:
                    levelStarList[i].gameObject.SetActive(false);
                    levelStarList[i].GetChild(0).gameObject.SetActive(false);
                    continue;
                case 1:
                    levelStarList[i].gameObject.SetActive(true);
                    levelStarList[i].GetChild(0).gameObject.SetActive(true);
                    levelStarList[i].GetChild(1).gameObject.SetActive(false);
                    levelStarList[i].GetChild(2).gameObject.SetActive(false);
                    break;
                case 2:
                    levelStarList[i].gameObject.SetActive(true);
                    levelStarList[i].GetChild(0).gameObject.SetActive(true);
                    levelStarList[i].GetChild(1).gameObject.SetActive(true);
                    levelStarList[i].GetChild(2).gameObject.SetActive(false);
                    break;
                case 3:
                    levelStarList[i].gameObject.SetActive(true);
                    levelStarList[i].GetChild(0).gameObject.SetActive(true);
                    levelStarList[i].GetChild(1).gameObject.SetActive(true);
                    levelStarList[i].GetChild(2).gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
    public void GetNextLevelName(string n)
    {
        Setting.NextLevel = n;
    }
    public void UnlockAll()
    {
        LevelProgession lvp = new LevelProgession();//only 10 level for now
        for (int i = 0; i < level.Count; i++)
        {
            lvp.level.Add(level[i].name);
        }
        SaveSystem.SaveLevelProgression(lvp);//keep at all cost
    }
}
