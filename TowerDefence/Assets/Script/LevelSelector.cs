using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButton;
    [SerializeField] private GameObject characterSelection;
    [SerializeField] private GameObject levelSelectionScreen;
    [SerializeField] private GameObject button; 
    private string nextlevel;

    public void DisableLevelSelectionScreen()
    {
       levelSelectionScreen.SetActive(false);
    }
    public void EnableLevelSelectionScreen()
    {
        levelSelectionScreen.SetActive(true);
    }
    public void DisableCharacterSelection()
    {
        characterSelection.SetActive(false);
    }
    public void EnableCharacterSelection() 
    {
        characterSelection.SetActive(true);
    }
    public void SelectMainMenu(string LvName)
    {
        SceneManager.LoadScene(LvName);
    } 
    public void Select(string LvName)
    {
        nextlevel = LvName;
        characterSelection.SetActive(true);
        levelSelectionScreen.SetActive(false);
    }
    public string LevelName()
    {
        return nextlevel;
    }
    public void LoadScence()
    {
        GlowingButton gb= button.GetComponent<GlowingButton>();
        gb.LoadLevel();
    }
    public void LoadScence(string LvName)
    {
        SceneManager.LoadScene(LvName);
    }
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
