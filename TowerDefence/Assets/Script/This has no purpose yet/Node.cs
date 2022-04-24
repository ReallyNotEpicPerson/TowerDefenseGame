using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color NoMoneyLeftColor;

    BoxCollider2D coll;

    public GameObject turret;
    [HideInInspector]
    public TurretBluePrint turretBluePrint;
    [HideInInspector]
    public bool isUpgraded = false;

    public Renderer rend;
    private Color ogColor;
    public Vector3 positionOffSet = new Vector3(1, 1, 1);

    BuildManager buildManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        ogColor = rend.material.color;
        coll = GetComponent<BoxCollider2D>();
        buildManager = BuildManager.instance;
    }
    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffSet;
    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (turret != null)
        {
            return;
        }
        if (!buildManager.CanBuild)
        {
            return;
        }
        BuildTurret(buildManager.GetTurretToBuild());
    }
    public void CallBuildManager()
    {
        buildManager.SelectNode(this);
    }
    public void BuildTurret(TurretBluePrint blueprint)
    {
        if (PlayerStat.moneyInGame < blueprint.cost)
        {
            //Debug.Log("Nah U poor!" + PlayerStat.money);
            return;
        }
        PlayerStat.moneyInGame -= blueprint.cost;
        
        GameObject _turret = Instantiate(blueprint.prefab, transform.position, Quaternion.identity);
        turret = _turret;
        turret.name = blueprint.prefab.name;
        turretBluePrint = blueprint;
        //turret.GetComponent<BaseTurretStat>().SetNode(this);

        GameObject Bfx = Instantiate(buildManager.BuildFX, transform.position, Quaternion.identity);
        Destroy(Bfx, 2f);
        coll.enabled = false;
        //Destroy(buildManager.tempTurret);
        //Debug.Log("Not so poor ay!:" + PlayerStat.money + "Dong Left");
    }
    public void UpgradeTurret()
    {
       /* if (PlayerStat.moneyInGame < turretBluePrint.upgradeCost)
        {
            Debug.Log("Nah U poor!,cant upgrade,lol" + PlayerStat.moneyInGame);
            return;
        }
        PlayerStat.moneyInGame -= turretBluePrint.upgradeCost;
        //old Turret go bye bye 
        Destroy(turret);
        //Make a new one 
        GameObject _turret = Instantiate(turretBluePrint.upgradePrefab, transform.position, Quaternion.identity);
        turret = _turret;
        //turret.GetComponent<BaseTurretStat>().SetNode(this);

        turret.name = turretBluePrint.upgradePrefab.name;*/

        GameObject Bfx = Instantiate(buildManager.BuildFX, transform.position, Quaternion.identity);
        Destroy(Bfx, 2f);

        //EnableState(NodeState.IsUpgraded);
        isUpgraded = true;
    }
    public void SellTurret()
    {
        PlayerStat.moneyInGame += turretBluePrint.GetSellAmount();
        GameObject Sellfx = Instantiate(buildManager.SellFX, transform.position, Quaternion.identity);
        Destroy(Sellfx, 2f);
       // turret.GetComponent<BaseTurretStat>().SetNode(null) ;
        Destroy(turret);

        turretBluePrint = null;
        isUpgraded = false;
        coll.enabled = true;
    }
    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!buildManager.CanBuild)
        {
            return;
        }
        if (buildManager.CheckIfGone() == true)
        {
            return;
        }
        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = NoMoneyLeftColor;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = ogColor;
    }
}
