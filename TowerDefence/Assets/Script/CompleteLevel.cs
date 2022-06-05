using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    public string nextLevel = "Level02";
    public int levelToUnlock = 2;

    public SceneFader sceneFader;
    private void OnValidate()
    {
        if (sceneFader == null)
        {
            sceneFader = FindObjectOfType<SceneFader>(true);
        }
    }
    public void Continue()
    {
        PlayerPrefs.SetInt("LevelReached", levelToUnlock);
        sceneFader.FadeTo(nextLevel);
    }
    public void Menu()
    {
        Time.timeScale = 1f;
        sceneFader.FadeTo(menuSceneName);
        //SceneManager.LoadScene(menuSceneName);
    }
}
