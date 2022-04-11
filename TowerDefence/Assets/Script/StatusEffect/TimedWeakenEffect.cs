using UnityEngine;

public class TimedWeakenEffect : TimedEffect
{
    private readonly Enemy _enemy;
    private readonly BaseTurretStat _turret;

    public TimedWeakenEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        obj.TryGetComponent(out _enemy);
        obj.TryGetComponent(out _turret);
    }
    public override void End()
    {
        _enemy.EndWeaken();
        _enemy.RemoveFX(Effect.ID);
    }
    protected override void ApplyEffect()
    {
        Weaken weaken = (Weaken)Effect;
        if (effectStacks == weaken.stackTime)
        {
            Debug.Log("Reaching maximum stack of " + weaken.stackTime);
            return;
        }
        if (effectStacks < weaken.stackTime && Effect.effectType.HasFlag(EffectType.StackingEffect))
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
        if (_enemy!=null)
        {
            _enemy.Weaken(weaken.extraDamageTaken);
            if (!_enemy.ContainFX(Effect.ID))
            {
                _enemy.AddFX(this);
            }
        }/*
        else if (_turret)
        {
            switch (_turret)
            {
                case BulletTypeTurret bulletTypeTurret:
                    bulletTypeTurret.IncreaseDamage(new StatModifier(weaken.extraDamageTaken.statValue.value,StatModType.PercentMult, this));
                    break;
                case LazerTypeTurret lazerTypeTurret:
                    //lazerTypeTurret.ReduceRate(new StatModifier(slowEffect._slowPercentage.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
        }*/
    }
}
