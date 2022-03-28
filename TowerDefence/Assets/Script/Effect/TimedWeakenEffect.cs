using UnityEngine;

public class TimedWeakenEffect : TimedEffect
{
    private readonly Enemy _enemy;

    public TimedWeakenEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void End()
    {
        _enemy.EndWeaken();
    }
    protected override void ApplyEffect()
    {
        Weaken weaken = (Weaken)effect;
        //tempPercentage = slowEffect._slowPercentage.value;
        if (effectStacks == weaken.stackTime)
        {
            Debug.Log("Reaching maximum stack of " + weaken.stackTime);
            return;
        }
        if (effectStacks < weaken.stackTime && effect.effectType.HasFlag(EffectType.StackingEffect))
        {
            effectStacks++;
            switch (weaken.increaseRate.modType)
            {
                case StatModType.Flat:
                    weaken.extraDamageTaken.statValue.AddingOneInstance(new StatModifier(weaken.increaseRate.statValue.value * effectStacks, StatModType.Flat, this));
                    break;
                case StatModType.PercentAdd:
                    weaken.extraDamageTaken.statValue.AddModifier(new StatModifier(weaken.increaseRate.statValue.value, StatModType.PercentAdd, this));
                    break;
                case StatModType.PercentMult:
                    weaken.extraDamageTaken.statValue.AddModifier(new StatModifier(weaken.increaseRate.statValue.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
            //Debug.Log("Stack num "+effectStacks+" Stack value " + slowEffect._slowPercentage.value);
        }
        _enemy.Weaken(weaken.extraDamageTaken);
    }
}
