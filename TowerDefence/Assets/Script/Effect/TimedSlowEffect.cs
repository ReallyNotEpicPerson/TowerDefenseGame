using System.Text;
using UnityEngine;

public class TimedSlowEffect : TimedEffect
{
    private readonly Enemy _enemy;
    private float tempPercentage = 0;

    public TimedSlowEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    #region [idk man ]
    public bool IsThisBetterThan(SlowEffect newSlowEffect)
    {
        SlowEffect ThisSlowEffect = (SlowEffect)effect;
        if (newSlowEffect._slowPercentage.value > ThisSlowEffect._slowPercentage.value)
        {
            return true;
        }
        return false;
    }
    public float BestDuration(SlowEffect newSlowEffect)
    {
        return Mathf.Max(newSlowEffect._duration, effect._duration);
    }
    #endregion
    protected override void ApplyEffect()
    {
        SlowEffect slowEffect = (SlowEffect)effect;
        //tempPercentage = slowEffect._slowPercentage.value;
        if (effectStacks == slowEffect.stackTime)
        {
            Debug.Log("Reaching maximum stack of " + slowEffect.stackTime);
            return;
        }
        if (effectStacks < slowEffect.stackTime && effect.effectType.HasFlag(EffectType.StackingEffect))
        {
            effectStacks++;
            switch (slowEffect.increaseRate.modType)
            {
                case StatModType.Flat:
                    slowEffect._slowPercentage.AddingOneInstance(new StatModifier(slowEffect.increaseRate.statValue.value*effectStacks, StatModType.Flat, this));
                    break;
                case StatModType.PercentAdd:
                    slowEffect._slowPercentage.AddModifier(new StatModifier(slowEffect.increaseRate.statValue.value, StatModType.PercentAdd, this));
                    break;
                case StatModType.PercentMult:
                    slowEffect._slowPercentage.AddModifier(new StatModifier(slowEffect.increaseRate.statValue.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
            //Debug.Log("Stack num "+effectStacks+" Stack value " + slowEffect._slowPercentage.value);
        }    
        _enemy.SlowDown(new StatModifier(slowEffect._slowPercentage.value, StatModType.PercentDebuffBest, this));


        //_enemy.AdjustSpeed(slowEffect._slowPercentage.value);
    }
    public override void End()
    {
        SlowEffect slowEffect = (SlowEffect)effect;
        slowEffect._slowPercentage.RemoveAllModifiersFromSource(this);
        _enemy.UndoModification(this);
        effectStacks = 0;
    }
    public override StringBuilder Display()
    {
        sb.Clear();
        sb.Append("Effect ID :" + effect.ID + "\n");
        sb.Append("SlowPtc :" + tempPercentage + "\n");
        sb.Append("Duration :" + _duration + "\n");
        sb.Append("Description :" + effect.description + "\n");
        sb.Append("/////////////////////////////////////////////////////////\n");
        return sb;
    }

}
