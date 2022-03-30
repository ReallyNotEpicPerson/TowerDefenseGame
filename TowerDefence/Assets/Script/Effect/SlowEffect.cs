using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slow Effect", menuName = "Slow")]
public class SlowEffect : BaseEffect
{
    public CharacterStat _slowPercentage;
    public StatValueType increaseRate;
    //public StatModType mod;
    //public float increaseRate;
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
        /*
        switch (mod)
        {
            case StatModType.None:
                break;
            case StatModType.Flat:
                break;
            case StatModType.PercentAdd:
                break;
            case StatModType.PercentMult:
                break;
            case StatModType.PercentDebuff:
                Debug.LogError("DONT");
                break;
            default:
                break;
        }*/
    }
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedSlowEffect(this, obj);
    }
}
