using UnityEngine;

public class Manual : MonoBehaviour
{
    public GameObject manualGo;
    public bool openAtStart = false;
    public GameObject[] content;
    private int page=0;
    public Setting setting;
    private float tempTimeScale;
    public static bool uiState = false;

    //public SceneFader sceneFader;
    private void Start()
    {
        uiState = false;
        page = 0;
        if (openAtStart && setting.OpenManualAtStart==false)
        {
            ShowManual();
        }
    }
    public void Next()
    {
        if (page == content.Length-1)
        {
            return;
        }
        page++;
        ShowPage();
    }
    public void Prev()
    {
        if (page == 0)
        {
            
            return;
        }
        page--;
        ShowPage();
    }
    public void ShowPage()
    {

        for (int i = 0; i < content.Length; i++)
        {
            if (page == i)
            {
                content[i].SetActive(true);
                continue;
            }
            content[i].SetActive(false);
        }
    }
    public void ShowManual()
    {
        manualGo.SetActive(true);
        ShowPage();
    }
    public void HideManual()
    {
        manualGo.SetActive(false);
        setting.OpenManualAtStart = true;
    }
    public void ShowManualStopInGame()
    {
        uiState = true;
        tempTimeScale = Time.timeScale;
        Time.timeScale = 0;
        manualGo.SetActive(true);
        ShowPage();
    }
    public void HideManualStopInGame()
    {
        uiState = false;
        Time.timeScale = tempTimeScale;
        manualGo.SetActive(false);
        setting.OpenManualAtStart = true;
    }
}
