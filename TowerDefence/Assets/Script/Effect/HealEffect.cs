using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealType
{
    Normal,
    Once,
    Percentage,
}
public class HealEffect : BaseEffect
{
    public HealType healType;
    public float HealAmount=0;
    public float rate=0;
    public float range=0;

    /*public void OnValidate()
    {
        if (healType.HasFlag(HealType.Percentage) && HealAmount > 1)
        {
            Debug.LogError("Choose the correct type BItCh!");
        }
    }*/
    public override TimedEffect Init(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}
