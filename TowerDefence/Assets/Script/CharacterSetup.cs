using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class CharacterSetup : MonoBehaviour
{
    private List<TurretBluePrint> character;
    public Transform lineUp;
    private int slotOpened;
    [SerializeField] private Button button;
    [SerializeField] private List<string> checkExistString;
    private Stack<int> checkExistInt;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button statButton;
    [SerializeField] private LevelSelector levelSelector;
    [SerializeField] private GameObject _dontDestroyOnLoad;

    void Start()
    {
        character = GameAsset.I.turret;
        slotOpened = 5;//change according to data
        GenerateButton1(slotOpened);
        GenerateButton2(GameAsset.I.turret.Count);
        checkExistString = new List<string>();
        checkExistInt = new Stack<int>();
        readyButton.interactable = false;
    }
    void GenerateButton1(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Button but = Instantiate(button);
            but.transform.SetParent(lineUp);
            but.transform.localScale = Vector3.one;
            but.name = "Button " + (i + 1);
            but.onClick.AddListener(delegate { Deselect(but); });
            but.interactable = false;
            //UnityEventTools.AddPersistentListener(delegate{ Deselect(but); });
        }
    }
    void GenerateButton2(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Button but = Instantiate(button);
            but.transform.SetParent(transform);
            but.transform.localScale = Vector3.one;
            but.name = "Button " + (i + 1);
            Image _image = Instantiate(GameAsset.I.charImg[i], but.transform.position, Quaternion.identity);
            //Image _image = Instantiate(character[i].prefabImage, but.transform.position, Quaternion.identity);
            _image.name = GameAsset.I.turret[i].prefab.name;
            _image.transform.SetParent(but.transform);
            _image.transform.localScale = Vector3.one;
            but.onClick.AddListener(delegate { Selected(but); });
        }
    }
    public void Selected(Button button)
    {
        for (int i = 0; i < lineUp.childCount; i++)
        {
            Button lineUpButton = lineUp.transform.GetChild(i).GetComponent<Button>();
            if (lineUpButton.interactable == false)
            {
                if (button.transform.childCount > 0 && checkExistString.Contains(button.transform.GetChild(0).name) == false)
                {
                    float duration = 3;
                    lineUpButton.interactable = true;
                    checkExistString.Add(button.transform.GetChild(0).name);
                    button.transform.GetChild(0).SetParent(lineUp.transform.GetChild(i));
                    RectTransform startPosition = lineUp.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
                    StartCoroutine(ButtonLerping(startPosition, Vector3.zero, duration));
                    checkExistInt.Push(button.transform.GetSiblingIndex());   
                    SetCharacterFormation.characterLineUp.Add(character[button.transform.GetSiblingIndex()]);
                    if (checkExistString.Count == slotOpened)
                    {
                        readyButton.interactable = true;
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
                continue;
        }
        return;
    }
    IEnumerator<WaitForSeconds> ButtonLerping(RectTransform a, Vector3 b, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            a.localPosition = Vector3.Lerp(a.localPosition, Vector3.zero, time);
            time += Time.deltaTime;
            yield return null;
        }
        a.localPosition = b;
    }
    public void Deselect(Button button)
    {
        string topButtonName = "";
        for (int i = lineUp.childCount - 1; i >= 0; i--)
        {
            if (lineUp.transform.GetChild(i).childCount > 0)
            {
                topButtonName = lineUp.GetChild(i).name;
                break;
            }
            else
            {
                continue;
            }
        }
        if (button.name != topButtonName)
        {
            //Debug.Log(topButtonName);
            return;
        }
        if (button.transform.GetChild(0) == null) { Debug.Log("wait this shouls not run"); return; }
        else
        {
            button.interactable = false;
            button.transform.GetChild(0).SetParent(transform.GetChild(checkExistInt.Peek()));
            StartCoroutine(ButtonLerping(transform.GetChild(checkExistInt.Peek()).GetChild(0).GetComponent<RectTransform>(), Vector3.zero, 3));
            SetCharacterFormation.characterLineUp.Remove(character[checkExistInt.Peek()]);
            checkExistString.Remove(transform.GetChild(checkExistInt.Peek()).GetChild(0).name);
            checkExistInt.Pop();
        }
        if (checkExistString.Count < slotOpened)
        {
            readyButton.interactable = false;
            return;
        }
    }
    public void ReturnButtonPressed()
    {
        levelSelector.DisableCharacterSelection();
    }
    public void readyButtonPressed()
    {
        DontDestroyOnLoad(_dontDestroyOnLoad);
        levelSelector.LoadScence();
    }
    /*void ShowStat()
    {
        //CharacterInfo characterInfo;
        //characterInfo.ShowStat();
    }
    void SortFunction()
    {
        character.Sort();
    }*/
}
