using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public TMP_Text normalUpgradeCost;
    public TMP_Text ultraUpgradeCost_1;
    public TMP_Text ultraUpgradeCost_2;
    public TMP_Text sellAmount;
    public Button normalUpgradeButton;
    public Button ultraUpgradeButton_1;
    public Button ultraUpgradeButton_2;

    public GameObject UpgradeStatPanel;
    public TMP_Text upgradeStat;

    public Vector3 offset;
    private RefTurret tempRefTurret;
    private BaseTurretStat upgrade;
    private BaseTurretStat present;

    [SerializeField] private TheBuildManager buildManager;

    public void Display(RefTurret target)
    {
        tempRefTurret = target;
        transform.position = target.referenceTurret.transform.position + offset;

        if (target.upgradeLevel <= target.refBlueprint.upgradePrefabs.Length)//check Upgradable
        {
            normalUpgradeButton.gameObject.SetActive(true);
            ultraUpgradeButton_1.gameObject.SetActive(false);
            ultraUpgradeButton_2.gameObject.SetActive(false);
            normalUpgradeCost.text = "$" + target.refBlueprint.upgradeCosts[target.upgradeLevel - 1];        
            normalUpgradeButton.interactable = true;

            sellAmount.text = "$" + target.refBlueprint.GetSellNormalAmount(target.upgradeLevel - 1);
            ui.SetActive(true);
        }
        else if (target.upgradeLevel > target.refBlueprint.upgradePrefabs.Length && target.UltraUpgradeLevel <= target.refBlueprint.ultraUpgrades.Length)
        {
            normalUpgradeButton.gameObject.SetActive(false);
            if (target.treeChoice == -1)
            {
                ultraUpgradeButton_1.gameObject.SetActive(true);
                ultraUpgradeButton_2.gameObject.SetActive(true);
                ultraUpgradeCost_1.text = "$" + target.refBlueprint.ultraUpgrades[0].ultraUpgradeCosts[target.UltraUpgradeLevel - 1];
                ultraUpgradeButton_1.interactable = true;
                ultraUpgradeCost_2.text = "$" + target.refBlueprint.ultraUpgrades[1].ultraUpgradeCosts[target.UltraUpgradeLevel - 1];
                ultraUpgradeButton_2.interactable = true;

                sellAmount.text = "$" + target.refBlueprint.GetSellNormalAmount(target.upgradeLevel - 1);
                ui.SetActive(true);
            }
            else if (target.treeChoice == 0)
            {
                ultraUpgradeButton_1.gameObject.SetActive(true);
                ultraUpgradeButton_2.gameObject.SetActive(false);
                ultraUpgradeCost_1.text = "$" + target.refBlueprint.ultraUpgrades[0].ultraUpgradeCosts[target.UltraUpgradeLevel - 1];
                ultraUpgradeButton_1.interactable = true;

                sellAmount.text = "$" + target.refBlueprint.ultraUpgrades[0].GetSellAmount(target.upgradeLevel - 1);
                ui.SetActive(true);
            }
            else if (target.treeChoice == 1)
            {
                ultraUpgradeButton_1.gameObject.SetActive(false);
                ultraUpgradeButton_2.gameObject.SetActive(true);
                ultraUpgradeCost_2.text = "$" + target.refBlueprint.ultraUpgrades[1].ultraUpgradeCosts[target.UltraUpgradeLevel - 1];
                ultraUpgradeButton_2.interactable = true;
                sellAmount.text = "$" + target.refBlueprint.ultraUpgrades[1].GetSellAmount(target.upgradeLevel - 1);
                ui.SetActive(true);
            }               
        }
        else
        {
            normalUpgradeButton.gameObject.SetActive(false);
            ultraUpgradeButton_1.gameObject.SetActive(false);
            ultraUpgradeButton_2.gameObject.SetActive(false);
            sellAmount.text = "$" + target.refBlueprint.GetSellNormalAmount(target.upgradeLevel - 1);
            ui.SetActive(true);
        }

    } 
    public void ShowUpgradePanel(int i)
    {        
        TurretStat(i);
        UpgradeStatPanel.SetActive(true);
    }
    public void HideUpgradePanel()
    {
        UpgradeStatPanel.SetActive(false);
        upgradeStat.text = "";
    }
    public void TurretStat(int treeIndex)
    {
        StringBuilder Stat = new StringBuilder();
        //upgradeCost.text = "$" + target.refBlueprint.upgradeCosts[target.upgradeLevel - 1];
        present = tempRefTurret.referenceTurret.GetComponentInChildren<BaseTurretStat>();

        if(tempRefTurret.upgradeLevel <= tempRefTurret.refBlueprint.upgradePrefabs.Length)
        {
            upgrade = tempRefTurret.refBlueprint.upgradePrefabs[tempRefTurret.upgradeLevel - 1].GetComponentInChildren<BaseTurretStat>();       
        }
        else if (tempRefTurret.upgradeLevel > tempRefTurret.refBlueprint.upgradePrefabs.Length && tempRefTurret.UltraUpgradeLevel <= tempRefTurret.refBlueprint.ultraUpgrades.Length)
        {
            if (tempRefTurret.treeChoice == -1)
            {
                upgrade = tempRefTurret.refBlueprint.ultraUpgrades[treeIndex].ultraUpgradePrefab[tempRefTurret.UltraUpgradeLevel- 1].GetComponentInChildren<BaseTurretStat>();
            }
            else
            {
                upgrade = tempRefTurret.refBlueprint.ultraUpgrades[tempRefTurret.treeChoice].ultraUpgradePrefab[tempRefTurret.UltraUpgradeLevel- 1].GetComponentInChildren<BaseTurretStat>();
            }
        }
        switch (upgrade)
        {
            case BulletTypeTurret upgradeVersion:
                Debug.Log("Yes it is");
                BulletTypeTurret presentBulletTurretVersion = present as BulletTypeTurret;
                Stat.Append(presentBulletTurretVersion.CompareTurretStat(upgradeVersion));
                break;
            case LazerTypeTurret upgradeVersion:
                LazerTypeTurret presentLazerTurretVersion = present as LazerTypeTurret;
                Stat.Append(presentLazerTurretVersion.TurretStat(upgradeVersion));
                break;
            default:
                Debug.Log("how?");
                break;
        }
        //Stat.Append("GEI");
        upgradeStat.text = Stat.ToString();
    }
    public void Upgrade(int i)
    {
        Debug.Log("Upgrade");
        buildManager.UltraUpgrade(tempRefTurret,i);
        ui.SetActive(false);
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
