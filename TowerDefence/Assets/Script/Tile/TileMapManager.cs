using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum TileNodeState
{
    None = 0,
    TurretPicked = 1,
    Upgrade_Sell = 2,
    ClickProp = 4,//?
    Events = 8,//?
}
public class RefTurret
{
    public TurretBluePrint refBlueprint;
    public GameObject referenceTurret;
    [Range(1, 6)]
    public int upgradeLevel = 0;
    public int treeChoice = -1;
    public int UltraUpgradeLevel = 0;

    public RefTurret()
    {
        refBlueprint = null;
        referenceTurret = null;
        upgradeLevel = 1;
        treeChoice = -1;
        UltraUpgradeLevel = 1;
}
    public RefTurret(TurretBluePrint bluePrint, GameObject turret)
    {
        refBlueprint = bluePrint;
        referenceTurret = turret;
        upgradeLevel = 1;
        treeChoice = -1;
        UltraUpgradeLevel = 1;
    }
    public RefTurret(TurretBluePrint bluePrint, GameObject turret, int level)
    {
        refBlueprint = bluePrint;
        referenceTurret = turret;
        upgradeLevel = level;
        treeChoice = -1;
        UltraUpgradeLevel = 1;
    }
    public bool HaveTurret { get { return refBlueprint != null || referenceTurret != null; } }

    public TurretBluePrint GetRef(out int level, out Vector3 pos)
    {
        level = upgradeLevel;
        pos = referenceTurret.transform.position;
        return refBlueprint;
    }
}

public class TileMapManager : MonoBehaviour
{
    private Camera Cam;
    //[SerializeField] private List<Tilemap> tilemapsList;//put tilemap in dict
    //private Dictionary<string, Tilemap> tilemapsDict;//idk yet
    [SerializeField] private Tilemap mainLayer;//Think About some Dict man :))
    [SerializeField] private Tilemap walkable;
    [SerializeField] private Tilemap prop;
    [SerializeField] private Tilemap highlight;

    public TileBase highlightTile;
    public Color hoverColor, notEnoughMoneyColor;
    public bool hasEnough = true;
    private bool selected = false;

    [SerializeField] private List<NodeTile> tileDatas;
    private Dictionary<TileBase, NodeTile> dataFromTile;
    public Dictionary<Vector3Int, RefTurret> turretOnTheField;

    [SerializeField] private TileNodeState state;
    private Vector3Int previousPosition;
    public Vector3Int localGridPos;

    [SerializeField] private Canvas canvas;
    bool CanvasMoved = false;

    [SerializeField] private GameObject nodeUI;

    private void Awake()
    {
        Cam = Camera.main;
        dataFromTile = new Dictionary<TileBase, NodeTile>();
        turretOnTheField = new Dictionary<Vector3Int, RefTurret>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTile.Add(tile, tileData);
            }
        }
        //buildManager = GetComponent<TheBuildManager>();  
    }
    private void Update()
    {
        if (IsMouseOverUI())
        {
            highlight.SetTile(previousPosition, null);
            SetTileColour(Color.white, previousPosition, mainLayer);
            return;
        }
        localGridPos = GetGridPosition(Input.mousePosition);
        if (state.HasFlag(TileNodeState.TurretPicked))
        {
            if (!CanvasMoved)
            {
                CanvasBeGone();
            }
            if (localGridPos == previousPosition || !mainLayer.HasTile(localGridPos) || walkable.HasTile(localGridPos) || prop.HasTile(localGridPos) || !dataFromTile.ContainsKey(mainLayer.GetTile(localGridPos)))
            {
                return;
            }
            highlight.SetTile(previousPosition, null);
            SetTileColour(Color.white, previousPosition, mainLayer);
            highlight.SetTile(localGridPos, highlightTile);
            if (hasEnough)
            {
                SetTileColour(hoverColor, localGridPos, mainLayer);
            }
            else
            {
                SetTileColour(notEnoughMoneyColor, localGridPos, mainLayer);
            }
            previousPosition = localGridPos;
            return;
        }
    }
    public void LateUpdate()
    {
        if (IsMouseOverUI() || state.HasFlag(TileNodeState.TurretPicked))
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if ((nodeUI.activeSelf && nodeUI.transform.position != Input.mousePosition) || !turretOnTheField.ContainsKey(localGridPos))
            {
                if (selected)
                {
                    nodeUI.SetActive(false);
                    selected = false;
                }
                return;
            }
            selected = true;
            if (state.HasFlag(TileNodeState.Upgrade_Sell))
            {
                turretOnTheField.TryGetValue(localGridPos, out RefTurret turret);
                if (!turret.HaveTurret)
                {
                    return;
                }
                Debug.Log("You Touch ME :3");
                NodeUI UI = nodeUI.GetComponent<NodeUI>();
                UI.Display(turret);
                //UI.ui.SetActive(true);
            }
            //Extract shiz from dict and do shit 
            EnableState(TileNodeState.Upgrade_Sell);
        }
        //if
    }
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    public void CanvasBeGone()
    {
        //RectTransform cv = 
        canvas.transform.GetChild(0).gameObject.SetActive(false);
        //Debug.Log(cv.sizeDelta);
        //cv.sizeDelta = new Vector2(cv.sizeDelta.x, cv.sizeDelta.y + 150);
        //Debug.Log(cv.sizeDelta);
        CanvasMoved = true;
    }
    public void CanvasComeBack()
    {
        //RectTransform cv = 
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        //Debug.Log(cv.sizeDelta);
        //cv.sizeDelta = new Vector2(cv.sizeDelta.x, cv.sizeDelta.y - 150);
        //Debug.Log(cv.sizeDelta);
        CanvasMoved = false;
    }
    public void ResetHoverColor()
    {
        highlight.SetTile(previousPosition, null);
        SetTileColour(Color.white, previousPosition, mainLayer);
    }
    #region grid pos Stuff
    public Vector3Int GetGridPosition(Vector3 pos)
    {
        return mainLayer.WorldToCell(Cam.ScreenToWorldPoint(pos));
    }
    public Vector3Int GetGridPositionP2(Vector3 pos)
    {
        return mainLayer.WorldToCell(pos);
    }
    public Vector3 GetWorldPosition(Vector3 pos)
    {
        return mainLayer.GetCellCenterWorld(GetGridPosition(pos));
    }
    #endregion
    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);
        // Set the colour.
        tilemap.SetColor(position, colour);
    }
    public void EnableState(TileNodeState state)
    {
        this.state = state;
    }
    public NodeTile GetNodeTile()
    {
        dataFromTile.TryGetValue(mainLayer.GetTile(localGridPos), out NodeTile nodeTile);
        return nodeTile;
    }
    public bool BuildTurret(TurretBluePrint blueprint)
    {
        if (PlayerStat.moneyInGame < blueprint.cost || AcceptablePosition())
        {
            Debug.Log("Nah U poor! or that spot is not okay!! >:-<");

            //call something that tell you poor af
            return false;
        }
        PlayerStat.moneyInGame -= blueprint.cost;
        GameObject _turret = Instantiate(blueprint.prefab, mainLayer.GetCellCenterWorld(localGridPos), Quaternion.identity);//nah , get from pool???
        RefTurret refTurret = new RefTurret(blueprint, _turret);
        turretOnTheField.Add(localGridPos, refTurret);
        //Debug.Log(localGridPos);

        _turret.name = blueprint.prefab.name;
        return true;
    }
    public bool AcceptablePosition2_0()
    {
        //maybe loop through the dict or sth then check all layer if there is an accceptable Position
        return true;
    }
    public bool AcceptablePosition()
    {
        return !mainLayer.HasTile(localGridPos)
            || walkable.HasTile(localGridPos)
            || prop.HasTile(localGridPos)
            || !dataFromTile.ContainsKey(mainLayer.GetTile(localGridPos))
            || turretOnTheField.ContainsKey(localGridPos)
            ;
    }
}
