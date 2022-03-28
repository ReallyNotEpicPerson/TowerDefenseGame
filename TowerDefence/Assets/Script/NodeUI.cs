using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public Text upgradeCost;
    public Text sellAmount;
    public Button upgradeButton;

    public Vector3 offset;
    private RefTurret tempRefTurret;

    [SerializeField] private TheBuildManager buildManager;

    public void Display(RefTurret target)
    {
        tempRefTurret = target;
        transform.position = target.referenceTurret.transform.position + offset;

        if (target.upgradeLevel <= target.refBlueprint.upgradedPrefabs.Length)//check Upgradable
        {
            upgradeCost.text = "$" + target.refBlueprint.upgradeCosts[target.upgradeLevel - 1];        
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCost.text = "Maxed";
            upgradeButton.interactable = false;
        }
        sellAmount.text = "$" + target.refBlueprint.GetSellAmount(target.upgradeLevel - 1);
        ui.SetActive(true);

    }
    public void Upgrade()
    {
        Debug.Log("Upgrade");
        buildManager.UpgradeTurret(tempRefTurret);
        ui.SetActive(false);
    }
    public void Sell()
    {
        buildManager.Sell(tempRefTurret);
        ui.SetActive(false);
    }
    /*
    public void DisableState(NodeState n)
    {
        nodeState &= ~n;
    }
    public void EnableState(NodeState n)
    {
        nodeState |= n;
    }*/
}
