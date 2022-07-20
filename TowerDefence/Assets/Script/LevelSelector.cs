using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject turretSelection;
    private GameObject levelSelectionScreen;
    [SerializeField] private GameObject button;
    public TMP_Text lvName;
    private string nextlevel;
    private string CurrentLevel;
    [SerializeField] private Image map;
    public Animator animator;
    public AudioSource audioSource;
    //private RectTransform recTrans;

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
        audioSource.Play();
        SceneManager.LoadScene(CurrentLevel);
    }
    public void LoadScence(string name)
    {
        audioSource.Play();
        SceneManager.LoadScene(name);
    }
    public void EnableTurretSelection(string text)//The one usisng right now
    {
        //SceneManager.LoadScene(LvName);
        CurrentLevel = text;
        //EnableTurretSelection();
        animator.SetTrigger("Play");
    }
    public void SetName(string text)
    {
        lvName.text = text;
    }
    public void SetIndex(int i)
    {
        GameAsset.I.formation.currentLevelIndex = i;
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
    public void PlayPopingSound()
    {
        audioSource.PlayOneShot(GameAsset.I.PopingSound);
    }
}
