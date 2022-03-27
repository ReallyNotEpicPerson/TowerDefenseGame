using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public Text roundsText;

    public SceneFader sceneFader;

    public string menuName= "MainMenu";

    public string level= "LevelSelection";
    void OnEnable()
    {
        roundsText.text = PlayerStat.rounds.ToString(); 
    }
    public void Retry()
    {
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        sceneFader.FadeTo(menuName);    
    }

    public void LevelSelection()
    {
        sceneFader.FadeTo(level);
    }
}
