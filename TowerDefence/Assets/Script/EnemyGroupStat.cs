using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class EnemyGroupStat//do not change anything too dramatic or else you die 
{
    public Sprite sprite;
    public GameObject enemy;
    [Range(1,200)]
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
}
