using UnityEngine;

[CreateAssetMenu(fileName = "Burn Effect", menuName = "Burn")]
public class DotsEffect : BaseEffect
{
    public CharacterStat damagePerRate;
    public CharacterStat rate;
    public StatValueType damageIncreaseRate;
    public StatValueType rateIncrease;//always negative mode
    //public StatModType damageMod;
    //public float damageIncreaseRate;
    //public StatModType rateMod;
    //public float rateIncrease;//always negative mode
    public int stackTime;

    public override void OnValidate()
    {
        base.OnValidate();
        if (effectType != EffectType.StackingEffect)
        {
            damageIncreaseRate.statValue.baseValue = 0;
            rateIncrease.statValue.baseValue = 0;
            stackTime = 1;
            return;
        }
        switch (damageIncreaseRate.modType)
        {
            case StatModType.None:
                Debug.LogError("Bruh, you need to choose");
                break;
            case StatModType.Flat:
                if (damageIncreaseRate.statValue.baseValue < 0)
                {
                    damageIncreaseRate.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentAdd:
                if (damageIncreaseRate.statValue.baseValue < 0)
                {
                    damageIncreaseRate.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentMult:
                if (damageIncreaseRate.statValue.baseValue < 0)
                {
                    damageIncreaseRate.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentDebuffBest:
                break;
            default:
                break;
        }
        switch (rateIncrease.modType)
        {
            case StatModType.None:
                Debug.LogError("Bruh, you need to choose");
                break;
            case StatModType.Flat:
                if (rateIncrease.statValue.baseValue > 0)
                {
                    rateIncrease.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentAdd:
                if (rateIncrease.statValue.baseValue > 0)
                {
                    rateIncrease.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentMult:
                if (rateIncrease.statValue.baseValue > 0)
                {
                    rateIncrease.statValue.baseValue = 0;
                }
                break;
            case StatModType.PercentDebuffBest:
                break;
            default:
                break;
        }
    }
    public override TimedEffect Init(GameObject obj)
    {
        return new TimeDotsEffect(this, obj);
    }
}