using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFearEffect : TimedEffect
{
    private readonly Enemy _enemy;
    //private EnemyMovement enemyMovement;
    public TimedFearEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void End()
    {
        //enemyMovement.Turn();
        _enemy.UndoModification(this);
        _enemy.TurnBack(1);
    }
    protected override void ApplyEffect()
    {
        FearEffect fear = (FearEffect)Effect;
        if (effectStacks < fear.stackTime && Effect.effectType.HasFlag(EffectType.StackingEffect))
        {
            effectStacks++;
            switch (fear.runBackSpeed.modType)
            {
                case StatModType.Flat:
                    fear.runBackSpeed.statValue.AddingOneInstance(new StatModifier(fear.runBackSpeed.statValue.value * effectStacks, StatModType.Flat, this));
                    break;
                case StatModType.PercentAdd:
                    fear.runBackSpeed.statValue.AddModifier(new StatModifier(fear.runBackSpeed.statValue.value, StatModType.PercentAdd, this));
                    break;
                case StatModType.PercentMult:
                    fear.runBackSpeed.statValue.AddModifier(new StatModifier(fear.runBackSpeed.statValue.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
            //Debug.Log("Stack num "+effectStacks+" Stack value " + slowEffect._slowPercentage.value);
        }
        _enemy.TurnBack(0);
        _enemy.IncreaseSpeed(new StatModifier(fear.runBackSpeed.statValue.value, fear.runBackSpeed.modType, this));
        
    }
}
