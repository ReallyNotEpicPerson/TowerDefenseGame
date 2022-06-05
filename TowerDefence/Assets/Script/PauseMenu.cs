using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject ui;

    public SceneFader sceneFader;
    public string mainMenuSceneName= "MainMenu";
    private float timeScale;
    public static bool uiState=false;

    private void Start()
    {
        uiState = false;
    }
    void Update()
    {
        if (Game_Managers.gameHasEnded)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            GameToggle();
        }
    }
    public void GameToggle()
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = timeScale;
        }
        uiState = ui.activeSelf;
    }

    public void Retry()
    {
        GameToggle();
        Time.timeScale = 1f;
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }
    public void Menu()
    {
        GameToggle();
        sceneFader.FadeTo(mainMenuSceneName);
        Time.timeScale = 1f;
    }
}
