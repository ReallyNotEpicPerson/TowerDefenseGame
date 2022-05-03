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
    void NewCreateButton()
    {
        for (int i = 0; i < GameAsset.I.turret.Count; i++)
        {
            Button temp = Instantiate(button);
            temp.transform.SetParent(parenting, false);
            temp.name = "Button " + i;
            TurretBluePrint tempTurret = GameAsset.I.turret[i];
            temp.onClick.AddListener(delegate { NewSelectTurret(tempTurret); });
            SetButtonText(temp, i);
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
        temp.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text += GameAsset.I.turret[index].cost.ToString() + " $";
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
