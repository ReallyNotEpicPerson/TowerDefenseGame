using UnityEngine;

[System.Serializable]
public class UltraUpgrade
{
    public GameObject[] ultraUpgradePrefab;
    public int[] ultraUpgradeCosts;
    [SerializeField] private float sellPtcReduction = 0.75f;

    public int GetSellAmount(int i)
    {
        //Debug.Log(i);
        if (i < ultraUpgradeCosts.Length)
        {
            if (sellPtcReduction > 1)
            {
                return (int)Mathf.Abs(ultraUpgradeCosts[i] * sellPtcReduction);
            }
            return (int)Mathf.Abs(ultraUpgradeCosts[i] * sellPtcReduction);
        }
        else
        {
            int costSum = SumOfCost();
            if (sellPtcReduction > 1)
            {
                return (int)Mathf.Abs(costSum * sellPtcReduction);
            }
            return (int)Mathf.Abs(costSum * sellPtcReduction);
        }
    }
    public int SumOfCost()
    {
        int costSum = 0;
        for (int j = 0; j < ultraUpgradeCosts.Length; j++)
        {
            costSum += ultraUpgradeCosts[j];
        }
        return costSum;
    }
    /*
    public void RedudeCost(float ptc)
    {
        cost = (int)Mathf.Ceil(cost * (1 - ptc));
        for (int i = 0; i < cost; i++)
        {
            upgradeCosts[i] = (int)Mathf.Ceil(upgradeCosts[i] * (1 - ptc));
        }
    }
    public void RedudeUpgradeCost(float ptc, int i)
    {
        upgradeCosts[i] = (int)Mathf.Ceil(upgradeCosts[i] * (1 - ptc));
    }*/
}

[System.Serializable]
public class TurretBluePrint
{
    public GameObject prefab;
    public int cost;

    [SerializeField] private float sellPtcReduction = 0.75f;
    public GameObject[] upgradePrefabs;
    public int[] upgradeCosts;

    public UltraUpgrade[] ultraUpgrades;

    public int GetNormalSellAmount(int index)
    {
        float amount = cost;
        for (int i = 0; i < index; i++)
        {
            amount += upgradeCosts[i];
        }
        return (int)Mathf.Abs(amount*=sellPtcReduction);
    }
    public int GetUltraUpgradeSellAmount(int index,int tree)
    {
        float amount = cost;
        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            amount += upgradeCosts[i];
        }
        for (int i = 0; i < index; i++)
        {
            amount += ultraUpgrades[tree].ultraUpgradeCosts[i];
        }
        return (int)Mathf.Abs(amount*= sellPtcReduction);
    }
    
    public void RedudeCost(float ptc)//Could be use in the future
    {
        cost = (int)Mathf.Ceil(cost * (1 - ptc));
        for (int i = 0; i < cost; i++)
        {
            upgradeCosts[i] = (int)Mathf.Ceil(upgradeCosts[i] * (1 - ptc));
        }
    }
    public void RedudeUpgradeCost(float ptc,int i)//Could be use in the future
    {
        upgradeCosts[i] = (int)Mathf.Ceil(upgradeCosts[i] * (1 - ptc));
    }
    public int GetSellAmount()
    {
        return cost / 2;
    }
}
