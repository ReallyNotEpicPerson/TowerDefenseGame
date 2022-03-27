using UnityEngine;

[System.Serializable]
public class TurretBluePrint
{
    public GameObject prefab;
    public int cost;
    public GameObject upgradePrefab;
    public int upgradeCost;

    [SerializeField] private float sellPtcReduction = 0.5f;
    public GameObject[] upgradedPrefabs;
    public int[] upgradeCosts;

    public int GetSellAmount(int i)
    {
        //Debug.Log(i);
        if (i < upgradeCosts.Length)
        {
            if (sellPtcReduction > 1)
            {
                return (int)Mathf.Abs(upgradeCosts[i] * sellPtcReduction);
            }
            return (int)Mathf.Abs(upgradeCosts[i] - upgradeCosts[i] * sellPtcReduction);
        }
        else
        {
            int costSum = SumOfCost();
            if (sellPtcReduction > 1)
            {
                return (int)Mathf.Abs(costSum * sellPtcReduction);
            }
            return (int)Mathf.Abs(costSum - costSum * sellPtcReduction);
        }
    }
    public int SumOfCost()
    {
        int costSum = 0;
        for (int j = 0; j < upgradeCosts.Length; j++)
        {
            costSum += upgradeCosts[j];
        }
        return costSum;
    }
    public int GetSellAmount()
    {
        return cost / 2;
    }
    public void RedudeCost(float ptc)
    {
        cost = (int)Mathf.Ceil(cost * (1 - ptc));
        for (int i = 0; i < cost; i++)
        {
            upgradeCosts[i] = (int)Mathf.Ceil(upgradeCosts[i] * (1 - ptc));
        }
    }
    public void RedudeUpgradeCost(float ptc)
    {
        upgradeCost = (int)Mathf.Ceil(upgradeCost * (1 - ptc));
    }
}
