using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelToLoad = "LevelSelection";

    public SceneFader sceneFader;

    public void Play()
    {
        sceneFader.FadeTo(levelToLoad);
    }

    public void Save()
    {

    }

    public void Quit()
    {
        Debug.Log("....exiting....");
        Application.Quit();
    }
}
