using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject turretSelection;
    private GameObject levelSelectionScreen;
    [SerializeField] private GameObject button; 
    private string nextlevel;
    private string CurrentLevel;
    [SerializeField] private Image map;

    public void SetMap(Sprite leMap)
    {
        map.sprite = leMap;
    }

    public void DisableLevelSelectionScreen()
    {
       levelSelectionScreen.SetActive(false);
    }
    public void EnableLevelSelectionScreen()
    {
        levelSelectionScreen.SetActive(true);
    }
    public void DisableTurretSelection()
    {
        turretSelection.SetActive(false);
    }
    public void EnableTurretSelection() 
    {
        turretSelection.SetActive(true);
    }
    public void SelectMainMenu(string LvName)
    {
        SceneManager.LoadScene(LvName);
    } 
    public void Select(string LvName)
    {
        nextlevel = LvName;
        turretSelection.SetActive(true);
        levelSelectionScreen.SetActive(false);
    }
    public string LevelName()
    {
        return nextlevel;
    }
    public void LoadScence()
    {
        SceneManager.LoadScene(CurrentLevel);
    }
    public void LoadScence(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void EnableTurretSelection(string LvName)//The one usisng right now
    {
        //SceneManager.LoadScene(LvName);
        CurrentLevel = LvName;
        EnableTurretSelection();
    }
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
