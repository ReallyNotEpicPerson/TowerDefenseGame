using System.Collections.Generic;
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
    private StatusEffectType flag;
    private PassiveAbility passiveAbility;
    public Animator animator;

    //private TMP_Text statusEffect;
    //private bool doneLerping = false;

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
        slotOpened = 5;//change according to data
        GenerateButton1(slotOpened);
        SetButton(GameAsset.I.turretSprite.Count);
        //GenerateButton2(GameAsset.I.charImg.Length);
        checkExistString = new List<string>();
        checkExistInt = new Stack<int>();
        readyButton.interactable = false;
    }
    public void SetButton(int num)
    {
        for (int i = 0; i < transform.childCount - 1; i++)
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
                if (GameAsset.I.turret[i].cost < 1)
                {
                    text.text = "Free";
                }
                else
                {
                    text.text = GameAsset.I.turret[i].cost + "$";
                }
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
            but.onClick.AddListener(delegate { GameAsset.I.PlayClickSound(); });
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
        turretToolTips.transform.Find("TurretName").GetComponent<TMP_Text>().text = turretStatsList[but.transform.GetSiblingIndex()].name;
        turretStat.parent.gameObject.transform.position = but.transform.position;


        if (turretStatsList[but.transform.GetSiblingIndex()] is BulletTypeTurret)
        {
            //Debug.Log("Is BulletTypeTurret");
            SupportTurretText.SetActive(false);
            SupportTurretIcon.SetActive(false);
            EnableState(StatusEffectType.RemoveAll);
            passiveAbility |= PassiveAbility.RemoveAll;
            BulletTypeTurret bulTurret = turretStatsList[but.transform.GetSiblingIndex()] as BulletTypeTurret;
            statText[0].text = bulTurret.GetDamage().ToString();
            statText[1].text = bulTurret.GetROF().ToString();
            statText[2].text = bulTurret.GetRange().ToString();
            Bullet bu = bulTurret.BulletStat();
            DisableState(StatusEffectType.RemoveAll);
            passiveAbility &= ~PassiveAbility.RemoveAll;
            for (int i = 3; i < statText.Count; i++)
            {
                if (bu.bulletType.HasFlag(StatusEffectType.SlowPerSecond) && !flag.HasFlag(StatusEffectType.SlowPerSecond))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetSlow().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.SlowPerSecond);
                    continue;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.Dots) && !flag.HasFlag(StatusEffectType.Dots))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetDOTS().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Dots);
                    continue;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.Fear) && !flag.HasFlag(StatusEffectType.Fear))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetFear().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Fear);
                    continue;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.Insta_Kill) && !flag.HasFlag(StatusEffectType.Insta_Kill))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetInstaKill().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Fear);
                    continue;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.Weaken) && !flag.HasFlag(StatusEffectType.Weaken))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetWeaken().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Weaken);
                    continue;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.ArmorBreaking) && !flag.HasFlag(StatusEffectType.ArmorBreaking))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = bulTurret.GetArmorBreaking().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = bulTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.ArmorBreaking);
                    continue;
                }
                else if (bulTurret.passiveAbility.HasFlag(PassiveAbility.CanShootWhenBuy) && !passiveAbility.HasFlag(PassiveAbility.CanShootWhenBuy))
                {
                    statText[i].text = "Cant be place on the field,but can shoot when bought";
                    statText[i].gameObject.SetActive(true);
                    Icons.transform.GetChild(i).gameObject.SetActive(false);
                    passiveAbility |= PassiveAbility.CanShootWhenBuy;
                }
                else if (bu.bulletType.HasFlag(StatusEffectType.PiercingShot) && !flag.HasFlag(StatusEffectType.PiercingShot))
                {
                    statText[i].text = "Ignore Ememy Armor";
                    statText[i].gameObject.SetActive(true);
                    Icons.transform.GetChild(i).gameObject.SetActive(false);
                    EnableState(StatusEffectType.PiercingShot);
                }
                else if (bulTurret.passiveAbility.HasFlag(PassiveAbility.QuadrupleDamage) && !passiveAbility.HasFlag(PassiveAbility.QuadrupleDamage))
                {
                    statText[i].text = "Have " + bulTurret.chanceToQuadrupleDamage + "% chance to quadruple damage";
                    statText[i].gameObject.SetActive(true);
                    Icons.transform.GetChild(i).gameObject.SetActive(false);
                    passiveAbility |= PassiveAbility.QuadrupleDamage;
                }
                else
                {
                    Icons.transform.GetChild(i).gameObject.SetActive(false);
                    statText[i].text = "";
                }

            }
            //StringBuilder text = bulTurret.GetStatusEffect();
        }
        else if (turretStatsList[but.transform.GetSiblingIndex()] is LazerTypeTurret)
        {
            //Debug.Log("Is LazerTypeTurret");
            SupportTurretText.SetActive(false);
            SupportTurretIcon.SetActive(false);
            EnableState(StatusEffectType.RemoveAll);
            LazerTypeTurret LazTurret = turretStatsList[but.transform.GetSiblingIndex()] as LazerTypeTurret;
            statText[0].text = LazTurret.GetDamage().ToString();
            statText[1].text = LazTurret.GetROF().ToString();
            statText[2].text = LazTurret.GetRange().ToString();
            DisableState(StatusEffectType.RemoveAll);
            for (int i = 3; i < statText.Count; i++)
            {
                if (LazTurret.bulletType.HasFlag(StatusEffectType.SlowPerSecond) && !flag.HasFlag(StatusEffectType.SlowPerSecond))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = LazTurret.GetSlow().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = LazTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.SlowPerSecond);
                    continue;
                }
                else if (LazTurret.bulletType.HasFlag(StatusEffectType.Dots) && !flag.HasFlag(StatusEffectType.Dots))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = LazTurret.GetDOTS().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = LazTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Dots);
                    continue;
                }
                else if (LazTurret.bulletType.HasFlag(StatusEffectType.Fear) && !flag.HasFlag(StatusEffectType.Fear))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = LazTurret.GetFear().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = LazTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Fear);
                    continue;
                }

                else if (LazTurret.bulletType.HasFlag(StatusEffectType.Weaken) && !flag.HasFlag(StatusEffectType.Weaken))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = LazTurret.GetWeaken().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = LazTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    EnableState(StatusEffectType.Weaken);
                    continue;
                }
                else if (LazTurret.bulletType.HasFlag(StatusEffectType.ArmorBreaking) && !flag.HasFlag(StatusEffectType.ArmorBreaking))
                {
                    statText[i].gameObject.SetActive(true);
                    statText[i].text = LazTurret.GetArmorBreaking().ToString();
                    Icons.transform.GetChild(i).GetComponent<Image>().sprite = LazTurret.StatusEffectSprite();
                    Icons.transform.GetChild(i).gameObject.SetActive(true);
                    statText[i].gameObject.SetActive(true);
                    EnableState(StatusEffectType.ArmorBreaking);
                    continue;
                }
                else
                {
                    Icons.transform.GetChild(i).gameObject.SetActive(false);
                    statText[i].text = "";
                }
            }
            /*else
            {
                turretToolTips.transform.Find("Stats").gameObject.SetActive(false);
                Icons.SetActive(false);
                SupportTurretText.SetActive(true);
                SupportTurretIcon.SetActive(true);

                //turretToolTips. transform.Find("Icon").gameObject.SetActive(false);
                //statText[0].text = "Increase Damage , Fire rate , Range ,  and Critical chance ";            
            }*/
        }
    }
    public void Hide()
    {
        turretStat.transform.parent.gameObject.SetActive(false);
        DisableState(StatusEffectType.RemoveAll);
        passiveAbility &= ~PassiveAbility.RemoveAll;
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
                    //doneLerping = false;
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
        //doneLerping = true;
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
            //doneLerping = false;
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
        animator.SetTrigger("Comeback");
        //levelSelector.DisableTurretSelection();
    }
    public void ReadyButtonPressed()
    {
        GameAsset.I.formation.SetLineUp(checkExistInt.ToArray());
        levelSelector.LoadScence();
    }
    public void DisableState(StatusEffectType bt)
    {
        flag &= ~bt;
    }
    public void EnableState(StatusEffectType bt)
    {
        flag |= bt;
    }
}
