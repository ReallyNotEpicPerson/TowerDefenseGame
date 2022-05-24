using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (PlayerStat.Lives <= 0 )
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
        gameHasEnded = true;
        completeLevelUI.SetActive(true);
    }
}
