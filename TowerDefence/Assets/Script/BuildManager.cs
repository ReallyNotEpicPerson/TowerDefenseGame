using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public NodeUI nodeUI;
    public NodeUI_2_For_Now newNodeUI;
    [HideInInspector]
    public GameObject tempTurret = null;

    private bool isGone = false;

    public StatUI_InGame statUI;
    //private TileMapManager tileMapManager;
    void Awake()
    {
        //tileMapManager = GetComponent<TileMapManager>();

        if (instance != null)
        {
            Debug.Log("2 build manager,Bad");
            return;
        }
        instance = this;
    }
    public GameObject BuildFX;
    public GameObject SellFX;

    private TurretBluePrint turretToBuild;
    private Node selectedNode=null;//change to turret some how

    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return PlayerStat.moneyInGame >= turretToBuild.cost; } }

    public void SelectTurretToBuild(TurretBluePrint turret)
    {
        turretToBuild = turret;
        //tileMapManager.EnableState(TileNodeState.TurretPicked);
        DeselectNode();
    }
    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }
        selectedNode = node;
        turretToBuild = null;
        nodeUI.SetTarget(node);
    }
    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }
    public TurretBluePrint GetTurretToBuild()
    {
        return turretToBuild;
    }
    public void MakingTempTurret(GameObject turret)
    {
        tempTurret = Instantiate(turret, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)), Quaternion.identity);
        tempTurret.SetActive(true);
        isGone = false;
    }
    public bool CheckIfGone()
    {
        return isGone;
    }
    void Update()
    {
        if (tempTurret != null)
        {
            tempTurret.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        }
        if (tempTurret != null && (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Escape)))
        {
            if(tempTurret.GetComponent<LazerTypeTurret>() != null)
            {
                tempTurret.GetComponent<LazerTypeTurret>().DestroyLeftoverUI();
            }
            isGone = true;
            Destroy(tempTurret);
            turretToBuild = null;
        }
    }
}
