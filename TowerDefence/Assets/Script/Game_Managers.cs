using UnityEngine;
using UnityEngine.UI;

public enum WinConditions
{
    FinsishAllWaves,
    Survive,
}
public class Game_Managers : MonoBehaviour
{
    public static bool gameHasEnded;
    public GameObject GameOverUI;
    public GameObject completeLevelUI;
    public Transform StarGained;
    private PlayerStat playerStat;
    public void OnValidate()
    {
        if (StarGained == null)
        {
            StarGained = completeLevelUI.transform.Find("StarsGained");
        }
    }
    public void Awake()
    {
        TryGetComponent(out playerStat);
    }
    void Start()
    {
        gameHasEnded = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameHasEnded)
        {
            return;
        }
        if (PlayerStat.Lives <= 0)
        {
            GameEnd();
        }
    }
    void GameEnd()
    {
        Time.timeScale = 1;
        gameHasEnded = true;
        GameOverUI.SetActive(true);
    }
    public void WinLevel()
    {
        Time.timeScale = 1;
        LevelProgession levelProgession = SaveSystem.LoadLevelProgression();
        if (!levelProgession.level.Contains(GameAsset.I.formation.NextLevel))
        {
            levelProgession.level.Add(GameAsset.I.formation.NextLevel);
        }
        int starNum = LiveToStar();
        if (starNum > levelProgession.star[GameAsset.I.formation.currentLevelIndex])
        {
            levelProgession.star[GameAsset.I.formation.currentLevelIndex] = starNum;
        }
        SaveSystem.SaveLevelProgression(levelProgession);
        if (starNum == 3)
        {
            StarGained.GetChild(0).GetComponent<Image>().color = Color.white;
            StarGained.GetChild(1).GetComponent<Image>().color = Color.white;
            StarGained.GetChild(2).GetComponent<Image>().color = Color.white;
        }
        else if (starNum == 2)
        {
            StarGained.GetChild(0).GetComponent<Image>().color = Color.white;
            StarGained.GetChild(1).GetComponent<Image>().color = Color.white;
        }
        else if (starNum == 1)
        {
            StarGained.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        gameHasEnded = true;
        completeLevelUI.SetActive(true);
    }
    
    public int LiveToStar()
    {
        float ptc = PlayerStat.Lives / playerStat.startlives;
        if (ptc == 1)
        {
            return 3;
        }
        if (ptc < 1 && ptc > .75)
        {
            return 2;
        }
        if (ptc > .25 && ptc < .75)
        {
            return 1;
        }
        return 0;
    } 
}
