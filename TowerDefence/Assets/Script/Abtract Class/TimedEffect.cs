using System.Text;
using UnityEngine;

public abstract class TimedEffect
{
    protected float _duration = 0;
    protected int effectStacks = 0;
    public BaseEffect Effect { get; }
    protected readonly GameObject Target;
    public bool IsFinished;

    protected StringBuilder sb = new StringBuilder();

    public TimedEffect(BaseEffect fx, GameObject tar)
    {
        Effect = fx;
        Target = tar;
    }
    #region Duration
    public float GetDuration()
    {
        return _duration;
    }
    public void ResetDuration()
    {
        _duration = Effect._duration;
        //Debug.Log(_duration);
    }
    public void IncreaseDuration(float dur)
    {
        _duration += dur;
        //Debug.Log(_duration);
    }
    public void ReduceDuration(float dur)
    {
        _duration -= dur;
    }
    #endregion
    public virtual void Tick()
    {
        if (Effect.expirableType.HasFlag(ExpirableType.NonExpireable))
        {
            return;
        }
        else if (Effect.expirableType.HasFlag(ExpirableType.Expirable))
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0)
            {
                End();
                IsFinished = true;
            }
        }
    }

    public virtual StringBuilder Display() // ill think about it,...(2days later),ye i need it
    {
        //sb.Clear();
        return sb;
    }
    /**
     * Activates buff or extends duration if ScriptableBuff has IsDurationStacked or IsEffectStacked set to true.
     */
    public void Activate()
    {//might redo this function
        if (Effect.effectType.HasFlag(EffectType.StackingEffect) || _duration <= 0) //activate shiz
        {
            ApplyEffect();
        }
        if (Effect.effectType.HasFlag(EffectType.StackableDuration) || _duration <= 0)//Add duration and shiz
        {
            IncreaseDuration(Effect._duration);
        }
        else if (Effect.effectType.HasFlag(EffectType.None) || _duration <= 0)//reset duration and shiz
        {
            ResetDuration();
        }
    }
    protected abstract void ApplyEffect();
    public abstract void End();
}
