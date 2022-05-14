using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class GlowingButton : MonoBehaviour
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private SpriteShapeController previous;
    [SerializeField] private GameObject LevelInfo;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject _dontDestroyOnLoad;
    [SerializeField] private string levelName;
    [SerializeField] private int rewardMoney;
    [SerializeField] private bool isLevel = false;
    Renderer rend;
    SpriteShapeController spriteShapeController;
    private Color NormalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        NormalColor = rend.material.color;
    }

    public void OpenLevelPath()
    {
        if (previous != null)
        {          
            previous.gameObject.SetActive(true);
        }
        else { Debug.Log("Sth went wrong") ; return; }
    }
    private void OnMouseDown()
    {
        Debug.Log("aaah,yamete!");
        //LevelInfo.SetActive(true);
        selectionScreen.SetActive(true);
        if (isLevel == true) { SetRewardMoney(); }

    }
    public void SetRewardMoney()
    {
        _dontDestroyOnLoad.GetComponent<CharacterFormation>().SetRewardMoney(rewardMoney);
    }
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        rend.material.color = hoverColor;
    }
    private void OnMouseExit()
    {
        rend.material.color = NormalColor;
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
    public void LoadPitStop()
    {
        SceneManager.LoadScene(levelName);
    }
    public string LevelName()
    {
        return levelName;
    }
}
