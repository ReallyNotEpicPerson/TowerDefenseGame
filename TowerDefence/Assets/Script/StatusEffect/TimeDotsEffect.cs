using System.Text;
using UnityEngine;

public class TimeDotsEffect : TimedEffect
{
    private readonly Enemy _enemy;
    private float tempRate = 0;
    private float timer = 0;
    private float tempDamage = 0;
    private float sumDamage = 0;

    public TimeDotsEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void Tick()
    {
        if (Effect.expirableType.HasFlag(ExpirableType.NonExpireable))
        {
            BurnPertick();
            return;
        }
        if (Effect.expirableType.HasFlag(ExpirableType.Expirable))
        {
            _duration -= Time.deltaTime;
            //Debug.Log(_duration);
            BurnPertick();
            if (_duration <= 0)
            {
                End();
                IsFinished = true;
            }
        }
    }
    public void BurnPertick()
    {
        timer -= Time.deltaTime;
        //Debug.Log(timer);
        if (timer <= 0)
        {
            if (Effect.ID.Contains("BU"))
            {
                if (!_enemy.TakeDamage(tempDamage, DamageDisplayerType.Burned))
                {
                    return;
                }
            }
            if (Effect.ID.Contains("POI"))
            {
                if (!_enemy.TakeDamage(tempDamage, DamageDisplayerType.Poisoned))
                {
                    return;
                }
            }
            if (Effect.ID.Contains("HEAL"))
            {
                if (!_enemy.TakeDamage(tempDamage, DamageDisplayerType.NO))
                {
                    return;
                }
            }
            //sumDamage += tempDamage;
            //Debug.Log(tempDamage);
            timer = tempRate;
        }
    }
    protected override void ApplyEffect()
    {
        DotsEffect burnEffect = (DotsEffect)Effect;
        if (effectStacks == burnEffect.stackTime)
        {
            Debug.Log("Reaching maximum stack of " + burnEffect.stackTime);
            return;
        }
        //Debug.Log("Burn Damage : " + burnEffect.damagePerRate.value + " Rate " + burnEffect.rate.value);
        tempDamage = burnEffect.damagePerRate.value;
        tempRate = burnEffect.rate.value;
        timer = tempRate;
        if (effectStacks < burnEffect.stackTime && Effect.effectType.HasFlag(EffectType.StackingEffect))
        {
            effectStacks++;
            switch (burnEffect.damageIncreaseRate.modType)
            {
                case StatModType.Flat:
                    burnEffect.damagePerRate.AddingOneInstance(new StatModifier(burnEffect.damageIncreaseRate.statValue.value * effectStacks, burnEffect.damageIncreaseRate.modType, this));
                    switch (burnEffect.rateIncrease.modType)
                    {
                        case StatModType.Flat:
                            burnEffect.rate.AddingOneInstance(new StatModifier(burnEffect.rateIncrease.statValue.value * effectStacks, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentAdd:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentMult:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                    }
                    break;
                case StatModType.PercentAdd:
                    //burnEffect.damagePerRate.AddingOneInstance(new StatModifier(burnEffect.damageIncreaseRate * effectStacks, burnEffect.damageMod, this));
                    burnEffect.damagePerRate.AddModifier(new StatModifier(burnEffect.damageIncreaseRate.statValue.value * effectStacks, burnEffect.damageIncreaseRate.modType, this));
                    switch (burnEffect.rateIncrease.modType)
                    {
                        case StatModType.Flat:
                            burnEffect.rate.AddingOneInstance(new StatModifier(burnEffect.rateIncrease.statValue.value * effectStacks, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentAdd:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentMult:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                    }
                    break;
                case StatModType.PercentMult:
                    //burnEffect.damagePerRate.AddingOneInstance(new StatModifier(burnEffect.damageIncreaseRate * effectStacks, burnEffect.damageMod, this));
                    burnEffect.damagePerRate.AddModifier(new StatModifier(burnEffect.damageIncreaseRate.statValue.value * effectStacks, burnEffect.damageIncreaseRate.modType, this));
                    switch (burnEffect.rateIncrease.modType)
                    {
                        case StatModType.Flat:
                            burnEffect.rate.AddingOneInstance(new StatModifier(burnEffect.rateIncrease.statValue.value * effectStacks, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentAdd:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                        case StatModType.PercentMult:
                            burnEffect.rate.AddModifier(new StatModifier(burnEffect.rateIncrease.statValue.value, burnEffect.rateIncrease.modType, this));
                            break;
                    }
                    break;
            }
        }
        if (Effect.ID.Contains("POI"))
        {
            _enemy.SetHealthColor(Color.green);
        }
        if (!_enemy.ContainFX(Effect.ID) && Effect.specialFX != null)
        {
            _enemy.AddFX(this);
        }
    }
    public override void End()
    {
        DotsEffect burnEffect = (DotsEffect)Effect;
        burnEffect.damagePerRate.RemoveAllModifiersFromSource(this);
        if (Effect.specialFX != null)
        {
            _enemy.RemoveFX(Effect.ID);
        }
        if (Effect.ID.Contains("POI"))
        {
            _enemy.SetHealthColor();
        }
        effectStacks = 0;
    }
    public override StringBuilder Display()
    {
        sb.Clear();
        sb.Append("Effect ID :" + Effect.ID + "\n");
        sb.Append("SumDamage :" + sumDamage + "\n");
        sb.Append("Damage :" + tempDamage + "\n");
        sb.Append("Rate :" + tempRate + "\n");
        sb.Append("Duration :" + _duration + "\n");
        sb.Append("Timer :" + timer + "\n");
        sb.Append("Description :" + Effect.description + "\n");
        sb.Append("/////////////////////////////////////////////////////////\n");
        return sb;
    }
}
