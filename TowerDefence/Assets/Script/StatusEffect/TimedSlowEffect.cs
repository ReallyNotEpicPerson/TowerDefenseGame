using System.Text;
using UnityEngine;

public class TimedSlowEffect : TimedEffect
{
    private readonly Enemy _enemy;
    private readonly BaseTurretStat _turret;
    private readonly float tempPercentage = 0;

    public TimedSlowEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        obj.TryGetComponent(out _enemy);
        obj.TryGetComponent(out _turret);
        //Debug.Log("WHICH "+_enemy + " " + _turret);
    }

    #region [idk man ]
    public bool IsThisBetterThan(SlowEffect newSlowEffect)
    {
        SlowEffect ThisSlowEffect = (SlowEffect)Effect;
        if (newSlowEffect._slowPercentage.statValue.value > ThisSlowEffect._slowPercentage.statValue.value)
        {
            return true;
        }
        return false;
    }
    public float BestDuration(SlowEffect newSlowEffect)
    {
        return Mathf.Max(newSlowEffect.duration, Effect.duration);
    }
    #endregion
    protected override void ApplyEffect()
    {
        SlowEffect slowEffect = (SlowEffect)Effect;
        if (effectStacks == slowEffect.stackTime)
        {
            if (_turret != null)
            {
                switch (_turret)
                {
                    case BulletTypeTurret bulletTypeTurret:
                        bulletTypeTurret.UndoRateModification(this);
                        break;
                    case LazerTypeTurret lazerTypeTurret:
                        lazerTypeTurret.UndoRateModification(this);
                        break;
                    default:
                        break;
                }
                effectStacks = 0;
            }
            //Debug.Log("Reaching maximum stack of " + slowEffect.stackTime);
            return;
        }
        if (effectStacks < slowEffect.stackTime && Effect.effectType.HasFlag(EffectType.StackingEffect))
        {
            effectStacks++;
            switch (slowEffect.increaseRate.modType)
            {
                case StatModType.Flat:
                    slowEffect._slowPercentage.statValue.AddingOneInstance(new StatModifier(slowEffect.increaseRate.statValue.value * effectStacks, StatModType.Flat, this));
                    break;
                case StatModType.PercentAdd:
                    slowEffect._slowPercentage.statValue.AddModifier(new StatModifier(slowEffect.increaseRate.statValue.value, StatModType.PercentAdd, this));
                    break;
                case StatModType.PercentMult:
                    slowEffect._slowPercentage.statValue.AddModifier(new StatModifier(slowEffect.increaseRate.statValue.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
            //Debug.Log("Stack num "+effectStacks+" Stack value " + slowEffect._slowPercentage.value);
        }
        if (_enemy != null)
        {
            switch (slowEffect._slowPercentage.modType)
            {
                case StatModType.Flat:
                    _enemy.SlowDown(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.Flat, this));
                    break;
                case StatModType.PercentAdd:
                    _enemy.SlowDown(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.PercentAdd, this));
                    break;
                case StatModType.PercentMult:
                    _enemy.SlowDown(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.PercentMult, this));
                    break;
                case StatModType.PercentDebuffBest:
                    _enemy.SlowDown(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.PercentDebuffBest, this));
                    break;
                default:
                    break;
            }
            if (slowEffect.ID.Contains("STUN"))
            {
                _enemy.EndCast();
            }
            if (Effect.specialFX != null && !_enemy.ContainFX(Effect.ID))
            {
                //Debug.Log("addFX");
                _enemy.AddFX(this);
            }
        }
        else if (_turret != null)
        {
            //Debug.Log(slowEffect._slowPercentage.value);
            switch (_turret)
            {
                case BulletTypeTurret bulletTypeTurret:
                    bulletTypeTurret.AddingOneInstanceRateMod(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.PercentMult, this));
                    break;
                case LazerTypeTurret lazerTypeTurret:
                    lazerTypeTurret.AddingOneInstanceRateMod(new StatModifier(slowEffect._slowPercentage.statValue.value, StatModType.PercentMult, this));
                    break;
                default:
                    break;
            }
        }
        //_enemy.AdjustSpeed(slowEffect._slowPercentage.value);
    }
    public override void End()
    {
        SlowEffect slowEffect = (SlowEffect)Effect;
        slowEffect._slowPercentage.statValue.RemoveAllModifiersFromSource(this);
        if (_enemy != null && Effect.specialFX!=null)
        {
            if (_enemy.enemyType.HasFlag(EnemyType.FirstHitSpeedBoost) && Effect.ID.Contains("ENE"))
            {
                Debug.Log("Yamete");
                _enemy.UndoSpeedBoost();
            }
            _enemy.UndoModification(this);
            _enemy.RemoveFX(Effect.ID);
        }
        else if (_turret != null)
        {
            switch (_turret)
            {
                case BulletTypeTurret bulletTypeTurret:
                    bulletTypeTurret.UndoRateModification(this);
                    break;
                case LazerTypeTurret lazerTypeTurret:
                    lazerTypeTurret.UndoRateModification(this);
                    break;
                default:
                    break;
            }
        }
        if (slowEffect.ID.Contains("STUN"))
        {
            _enemy.ResumeCast();
        }
        effectStacks = 0;
    }
    public override StringBuilder Display()
    {
        sb.Clear();
        sb.Append("Effect ID :" + Effect.ID + "\n");
        sb.Append("SlowPtc :" + tempPercentage + "\n");
        sb.Append("Duration :" + _duration + "\n");
        sb.Append("Description :" + Effect.description + "\n");
        sb.Append("/////////////////////////////////////////////////////////\n");
        return sb;
    }

}
