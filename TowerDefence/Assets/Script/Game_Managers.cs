using UnityEngine;
using UnityEngine.SceneManagement;

public enum WinConditions
{
    FinsishAllWaves,
    

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
        if (PlayerStat.Lives <= 0 || Input.GetKey(KeyCode.O))
        {
            GameEnd();
        }
    }
    void GameEnd()
    {
        gameHasEnded = true;
        GameOverUI.SetActive(true);
    }
    public void WinLevel()
    {
        gameHasEnded = true;
        completeLevelUI.SetActive(true);
    }
}
