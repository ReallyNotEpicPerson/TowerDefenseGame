using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<TurretBluePrint> turret;
    [SerializeField] private Button button;
    [SerializeField] private Transform parenting;

    //BuildManager buildManager;

    public TheBuildManager theBuildManager;

    //public StatUI statUI;

    public void Awake()
    {
        NewCreateButton();
    }
    /*void Start()
    {
        //buildManager = BuildManager.instance;

        //CreateButton();
    }*/
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)|| Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("Yes");
            NewSelectTurret(GameAsset.I.turret[GameAsset.I.formation.characterLineUp[0]]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            NewSelectTurret(GameAsset.I.turret[GameAsset.I.formation.characterLineUp[1]]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            NewSelectTurret(GameAsset.I.turret[GameAsset.I.formation.characterLineUp[2]]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            NewSelectTurret(GameAsset.I.turret[GameAsset.I.formation.characterLineUp[3]]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            NewSelectTurret(GameAsset.I.turret[GameAsset.I.formation.characterLineUp[4]]);
        }
    }
    void NewCreateButton()
    {
        for (int i = 0; i < GameAsset.I.formation.characterLineUp.Length; i++)
        {
            Button temp = Instantiate(button);
            temp.transform.SetParent(parenting, false);
            temp.name = "Button " + i;
            TurretBluePrint tempTurret = GameAsset.I.turret[GameAsset.I.formation.characterLineUp[i]];
            temp.onClick.AddListener(delegate { NewSelectTurret(tempTurret); });
            temp.transform.GetChild(0).GetComponent<Image>().sprite = GameAsset.I.turretSprite[GameAsset.I.formation.characterLineUp[i]];
            SetButtonText(temp, GameAsset.I.formation.characterLineUp[i]);
            temp.onClick.AddListener(delegate { PlaySound(); });
        }
    }
    void CreateButton()
    {
        for (int i = 0; i < turret.Count; i++)
        {
            Button temp = Instantiate(button);
            //Image img = Instantiate(turret[i].prefabImage);
            temp.transform.SetParent(parenting, false);
            //img.transform.SetParent(temp.transform,false);
            //img.rectTransform.localPosition = Vector3.zero;
            temp.name = "Button " + i;
            TurretBluePrint tempTurret = turret[i];
            temp.onClick.AddListener(delegate { NewSelectTurret(tempTurret); });
            SetButtonText(temp, i);
        }
    }
    void SetButtonText(Button temp, int index)
    {
        temp.transform.GetChild(1).GetComponent<TMP_Text>().text = GameAsset.I.turret[index].prefab.name;
        if (GameAsset.I.turret[index].cost < 1)
        {
            temp.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text += "Free";
        }
        else
        {
            temp.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text += GameAsset.I.turret[index].cost.ToString() + " $";
        }
    }
    public void NewSelectTurret(TurretBluePrint turretBluePrint)
    {
        Debug.Log(turretBluePrint.prefab.name + " selected");
        if (theBuildManager.tempTurret != null)
        {
            Destroy(theBuildManager.tempTurret);
            theBuildManager.TempTurret(turretBluePrint);
        }
        else
        {
            theBuildManager.TempTurret(turretBluePrint);
        }
        theBuildManager.SelectTurretToBuild(turretBluePrint);
        //buildManager.SelectTurretToBuild(turretBluePrint);
    }
    public void PlaySound()
    {
        GameAsset.I.audioSource.PlayOneShot(GameAsset.I.clickClack);
    }
    /*
    public void SelectTurret(TurretBluePrint Character)
    {
        Debug.Log(Character.prefab.name + " selected");
        if (buildManager.tempTurret != null)
        {
            Destroy(buildManager.tempTurret);
            buildManager.MakingTempTurret(Character.prefab);
        }
        else
        {
            buildManager.MakingTempTurret(Character.prefab);
        }
        buildManager.SelectTurretToBuild(Character);
        //statUI.TransferCharacter(StandardTurret.prefab);
    }*/
}
