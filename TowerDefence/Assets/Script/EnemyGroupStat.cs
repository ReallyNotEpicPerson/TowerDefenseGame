using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyGroupStat 
{
    public GameObject enemy;
    [Range(1,1000)]
    public int count;
    public float rate;
    [Range(0, 4)]
    public int path;
    public float delayBetweenGroup;
}
