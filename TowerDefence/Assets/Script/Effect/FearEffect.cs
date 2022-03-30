using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fear Effect", menuName = "Fear")]
public class FearEffect : BaseEffect
{
    public StatValueType runBackSpeed;
    public int stackTime;

    public override void OnValidate()
    {
        base.OnValidate();
        if (!effectType.HasFlag(EffectType.StackingEffect))
        {
            stackTime = 1;
        }
    }
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedFearEffect(this,obj);
    }
}
