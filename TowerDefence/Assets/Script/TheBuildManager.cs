using UnityEngine;

public enum BuildManagerState
{
    None = 0,
    BuildTurret = 1,
}
public class TheBuildManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject tempTurret = null;

    [SerializeField] private BuildManagerState state;
    private TileMapManager tileMapManager;

    public GameObject BuildFX;
    public GameObject UpgradeEffect;
    public GameObject SellFX;

    private TurretBluePrint turretToBuild;
    [SerializeField] private Camera cam;
    void Awake()
    {
        tileMapManager = GetComponent<TileMapManager>();
        cam = Camera.main;
    }
    public bool MoneyState { get { return PlayerStat.moneyInGame >= turretToBuild.cost; } }
    public void SelectTurretToBuild(TurretBluePrint turret)
    {
        turretToBuild = turret;
        tileMapManager.EnableState(TileNodeState.TurretPicked);
        EnableState(BuildManagerState.BuildTurret);
    }
    public void TempTurret(TurretBluePrint turret)
    {
        tempTurret = Instantiate(turret.prefab, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane)), Quaternion.identity);
        tempTurret.SetActive(true);
        //tempTurret.transform.GetSiblingIndex;
    }
    private void Update()
    {
        if (!state.HasFlag(BuildManagerState.BuildTurret) || TileMapManager.IsMouseOverUI())
        {
            return;
        }
        //tempTurret.transform.position = tileMapManager.GetWorldPosition(Input.mousePosition);
        tempTurret.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        tileMapManager.hasEnough = MoneyState;
        if (Input.GetMouseButtonDown(0))
        {
            if (!tileMapManager.BuildTurret(turretToBuild))
            {
                return; 
            }
            Destroy(Instantiate(UpgradeEffect, tileMapManager.GetWorldPosition(Input.mousePosition), Quaternion.identity), 2f);
            Destroy(tempTurret);
            turretToBuild = null;
            tileMapManager.EnableState(TileNodeState.None);
            tileMapManager.ResetHoverColor();
            EnableState(BuildManagerState.None);
            tileMapManager.CanvasComeBack();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Destroy(tempTurret);
            turretToBuild = null;
            tileMapManager.EnableState(TileNodeState.None);
            tileMapManager.ResetHoverColor();
            EnableState(BuildManagerState.None);
            tileMapManager.CanvasComeBack();
        }
    }
    public void UpgradeTurret(RefTurret refTurret)
    {
        if (PlayerStat.moneyInGame < refTurret.refBlueprint.upgradeCosts[refTurret.upgradeLevel - 1])
        {
            Debug.Log("Nah U poor!,cant upgrade,lol" + PlayerStat.moneyInGame);
            return;
        }
        PlayerStat.moneyInGame -= refTurret.refBlueprint.upgradeCosts[refTurret.upgradeLevel - 1];
        //old Turret go bye bye 
        GameObject upgradePrefab = Instantiate(refTurret.refBlueprint.upgradedPrefabs[refTurret.upgradeLevel - 1], refTurret.referenceTurret.transform.position, Quaternion.identity);
        Destroy(refTurret.referenceTurret);
        refTurret.referenceTurret = upgradePrefab;
        refTurret.referenceTurret.name = upgradePrefab.name;
        refTurret.upgradeLevel++;
        Debug.Log(refTurret.upgradeLevel + " " + refTurret.refBlueprint.upgradeCosts.Length);
        GameObject Bfx = Instantiate(UpgradeEffect, upgradePrefab.transform.position, Quaternion.identity);
        Destroy(Bfx, 2f);
    }

    public void Sell(RefTurret refTurret)
    {
        PlayerStat.moneyInGame += refTurret.refBlueprint.GetSellAmount(refTurret.upgradeLevel - 1);
        GameObject Sellfx = Instantiate(SellFX, transform.position, Quaternion.identity);
        Destroy(Sellfx, 2f);

        //Debug.Log(tileMapManager.turretOnTheField.TryGetValue(tileMapManager.GetGridPositionP2(refTurret.referenceTurret.transform.position),out _) + " " + tileMapManager.GetGridPositionP2(refTurret.referenceTurret.transform.position));
        tileMapManager.turretOnTheField.Remove(tileMapManager.GetGridPositionP2(refTurret.referenceTurret.transform.position));
        Destroy(refTurret.referenceTurret);
    }

    public void EnableState(BuildManagerState state)
    {
        this.state = state;
    }
}