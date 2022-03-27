using UnityEngine;
using UnityEngine.UI;
public class NodeUI : MonoBehaviour
{
    public GameObject ui;

    public Text upgradeCost;

    public Text sellAmount;

    public Button upgradeButton;

    private Node target;//bye

    private GameObject _Target;

    public void SetTarget(Node _target)
    {
        target = _target;
        transform.position = _target.GetBuildPosition();//Set NodeUI position
        if (!target.isUpgraded)//check Upgradable
        {
            upgradeCost.text ="$"+ target.turretBluePrint.upgradeCost;
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCost.text = "Maxed";
            upgradeButton.interactable = false;
        }

        sellAmount.text = "$" + target.turretBluePrint.GetSellAmount();
        
        ui.SetActive(true);//show up now boi
    }
    public void Hide()
    {
        ui.SetActive(false);
    }
    public void Upgrade() 
    {
        target.UpgradeTurret();
        BuildManager.instance.DeselectNode();
    }
    public void Sell ()
    {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
    }
}
