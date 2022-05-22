using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class TurretSetup : MonoBehaviour
{
    //private List<TurretBluePrint> character;
    public Transform lineUp;
    private int slotOpened;
    [SerializeField] private Button button;
    [SerializeField] private List<string> checkExistString;
    private Stack<int> checkExistInt;
    [SerializeField] private Button readyButton;
    [SerializeField] private LevelSelector levelSelector;
    [SerializeField] private GameObject turretToolTips;
    [SerializeField] private GameObject Icons;
    [SerializeField] private GameObject SupportTurretText;
    [SerializeField] private GameObject SupportTurretIcon;

    [SerializeField] private Transform turretStat;
    private List<BaseTurretStat> turretStatsList = new List<BaseTurretStat>();
    private Dictionary<BaseTurretStat, BaseBulletClass> bullet = new Dictionary<BaseTurretStat, BaseBulletClass>();
    private List<TMP_Text> statText = new List<TMP_Text>();

    private bool doneLerping = false;

    void Start()
    {
        //character = GameAsset.I.turret;
        for (int i = 0; i < turretStat.transform.childCount; i++)
        {
            statText.Add(turretStat.GetChild(i).GetComponent<TMP_Text>());
        }
        for (int i = 0; i < GameAsset.I.turret.Count; i++)
        {
            BaseTurretStat baseTurret = GameAsset.I.turret[i].prefab.GetComponentInChildren<BaseTurretStat>();
            turretStatsList.Add(baseTurret);
            if (baseTurret is BulletTypeTurret)
            {
                BulletTypeTurret pewpewpew = baseTurret as BulletTypeTurret;
                bullet.Add(baseTurret, pewpewpew.BulletPrefab.GetComponent<BaseBulletClass>());
            }
        }
        slotOpened = 4;//change according to data
        GenerateButton1(slotOpened);
        SetButton(GameAsset.I.turretSprite.Count);
        //GenerateButton2(GameAsset.I.charImg.Length);
        checkExistString = new List<string>();
        checkExistInt = new Stack<int>();
        readyButton.interactable = false;
    }
    public void SetButton(int num)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < num)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                Image _image = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
                _image.sprite = GameAsset.I.turretSprite[i];
                _image.name = GameAsset.I.turret[i].prefab.name;
                //_image.transform.SetParent(transform.GetChild(i).transform);
                //_image.transform.localScale = Vector3.one;
                TMP_Text text = _image.GetComponentInChildren<TMP_Text>();
                text.text = GameAsset.I.turret[i].cost + "$";
            }
            else
            {
                transform.GetChild(i).GetComponent<Button>().interactable = false;
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
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
            //but.GetComponent<Event>().ad(delegate { Deselect(but); });

            //UnityEventTools.AddPersistentListener(delegate{ Deselect(but); });
        }
    }
    public void StatHovering(Button but)
    {
        if (but.transform.childCount == 0 || !but.transform.GetChild(0).gameObject.activeSelf)
        {
            return;
        }
        //turretToolTips.transform.Find("Icon").gameObject.SetActive(true);
        turretToolTips.SetActive(true);
        turretToolTips.transform.GetChild(0).GetComponent<TMP_Text>().text = turretStatsList[but.transform.GetSiblingIndex()].name;
        turretStat.parent.gameObject.transform.position = but.transform.position;


        if (turretStatsList[but.transform.GetSiblingIndex()] is BulletTypeTurret)
        {
            //Debug.Log("Is BulletTypeTurret");
            SupportTurretText.SetActive(false);
            SupportTurretIcon.SetActive(false);

            BulletTypeTurret bulTurret = turretStatsList[but.transform.GetSiblingIndex()] as BulletTypeTurret;
            statText[0].text = bulTurret.GetDamage().ToString();
            statText[1].text = bulTurret.GetROF().ToString();
            statText[2].text = bulTurret.GetRange().ToString();
            StringBuilder text = bulTurret.GetStatusEffect();
            //statText[3].text = ;
        }
        else if (turretStatsList[but.transform.GetSiblingIndex()] is LazerTypeTurret)
        {
            //Debug.Log("Is LazerTypeTurret");
            SupportTurretText.SetActive(false);
            SupportTurretIcon.SetActive(false);
            LazerTypeTurret LazTurret = turretStatsList[but.transform.GetSiblingIndex()] as LazerTypeTurret;
            statText[0].text = LazTurret.GetDamage().ToString();
            statText[1].text = LazTurret.GetROF().ToString();
            statText[2].text = LazTurret.GetRange().ToString();
            statText[3].text = LazTurret.GetStatusEffect().ToString();
        }
        else
        {
            turretToolTips.transform.Find("Stats").gameObject.SetActive(false);
            Icons.SetActive(false);
            SupportTurretText.SetActive(true);
            SupportTurretIcon.SetActive(true);

            //turretToolTips. transform.Find("Icon").gameObject.SetActive(false);
            //statText[0].text = "Increase Damage , Fire rate , Range ,  and Critical chance ";            
        }
    }
    public void Hide()
    {
        turretStat.transform.parent.gameObject.SetActive(false);
    }
    public void Selected(Button button)
    {
        for (int i = 0; i < lineUp.childCount; i++)
        {
            Button lineUpButton = lineUp.transform.GetChild(i).GetComponent<Button>();
            if (lineUpButton.interactable == false)
            {
                if (button.transform.childCount > 0 && checkExistString.Contains(button.transform.GetChild(0).GetChild(0).name) == false)
                {
                    float duration = 3;
                    lineUpButton.interactable = true;
                    checkExistString.Add(button.transform.GetChild(0).GetChild(0).name);
                    button.transform.GetChild(0).SetParent(lineUp.transform.GetChild(i));
                    RectTransform startPosition = lineUp.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
                    doneLerping = false;
                    StartCoroutine(ButtonLerping(startPosition, Vector3.zero, duration));
                    checkExistInt.Push(button.transform.GetSiblingIndex());
                    //SetCharacterFormation.characterLineUp.Add(character[button.transform.GetSiblingIndex()]);
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
        doneLerping = true;
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
        if (button.transform.GetChild(0) == null) { Debug.LogError("wait this shouls not run"); return; }
        else
        {
            button.interactable = false;
            button.transform.GetChild(0).SetParent(transform.GetChild(checkExistInt.Peek()));
            doneLerping = false;
            StartCoroutine(ButtonLerping(transform.GetChild(checkExistInt.Peek()).GetChild(0).GetComponent<RectTransform>(), Vector3.zero, 3));
            //SetCharacterFormation.characterLineUp.Remove(character[checkExistInt.Peek()]);
            checkExistString.Remove(transform.GetChild(checkExistInt.Peek()).GetChild(0).GetChild(0).name);
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
        levelSelector.DisableTurretSelection();
    }
    public void ReadyButtonPressed()
    {
        levelSelector.LoadScence();
        GameAsset.I.formation.SetLineUp(checkExistInt.ToArray());
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
