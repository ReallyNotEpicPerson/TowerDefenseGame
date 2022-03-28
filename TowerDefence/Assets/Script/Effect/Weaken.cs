using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weaken Effect", menuName = "Weaken")]
public class Weaken : BaseEffect
{   
    public StatValueType extraDamageTaken;
    public StatValueType increaseRate;
    public int stackTime;

    public override void OnValidate()
    {
        base.OnValidate();
        if (!effectType.HasFlag(EffectType.StackingEffect))
        {
            increaseRate.statValue.baseValue = 0;
            stackTime = 1;
            increaseRate.modType |= StatModType.None;
        }
        if (increaseRate.statValue.value > 1)
        {
            increaseRate.statValue.baseValue = 0;
        }
        if (extraDamageTaken.modType!=StatModType.Flat && extraDamageTaken.statValue.baseValue > 1)
        {
            extraDamageTaken.statValue.baseValue = 0;
        }
    }

    public override TimedEffect init(GameObject obj)
    {
        return new TimedWeakenEffect(this,obj);
    }
}
