using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class EnemyGroupStat//do not change anything too dramatic or else you die 
{
    public Sprite sprite;
    public GameObject enemy;
    [Range(1, 200)]
    [FormerlySerializedAs("count")]
    public int count;
    public bool randomRateRange;
    [FormerlySerializedAs("rate")]
    public float startRate;
    public float endRate;
    [Range(0, 4)]
    public int path;
    public bool useNavmesh = false;
    public float delayBetweenGroup;


    public EnemyGroupStat(EnemyGroupStat stat)
    {        
        sprite = null;
        enemy = null;
        count = stat.count;
        randomRateRange = stat.randomRateRange;
        startRate = stat.startRate;
        endRate = stat.path;
        path = stat.path;
        useNavmesh = stat.useNavmesh;
        delayBetweenGroup = stat.delayBetweenGroup;
    }
    public EnemyGroupStat()
    {
        sprite = null;
        enemy = null;
        count = 1;
        randomRateRange = false;
        startRate = 0.5f;
        endRate = 0;
        path = 0;
        useNavmesh = false;
        delayBetweenGroup = 0;
    }
}
