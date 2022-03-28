using System.Text;
using UnityEngine;

public class TimedBurnEffect : TimedEffect
{
    private readonly Enemy _enemy;
    private float tempRate = 0;
    private float timer = 0;
    private float tempDamage = 0;
    private float sumDamage = 0;

    public TimedBurnEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void Tick()
    {
        if (effect.expirableType.HasFlag(ExpirableType.NonExpireable))
        {
            burnPertick();
            return;
        }            
        if (effect.expirableType.HasFlag(ExpirableType.Expirable))
        {        
            _duration -= Time.deltaTime;
            //Debug.Log(_duration);
            burnPertick();
            if (_duration <= 0)
            {
                End();
                IsFinished = true;
            }
        }
        
    }
    public void burnPertick()
    {
        timer -= Time.deltaTime;
        //Debug.Log(timer);
        if (timer <= 0)
        {
            if(!_enemy.TakeDamage(tempDamage, DamageDisplayerType.Burned))
            {
                return;
            }
            //sumDamage += tempDamage;
            _enemy.EffectColor(Color.yellow);
            //Debug.Log(tempDamage);
            timer = tempRate;
        }
    }
    protected override void ApplyEffect()
    {
        BurnEffect burnEffect = (BurnEffect)effect;
        if(effectStacks == burnEffect.stackTime)
        {
            Debug.Log("Reaching maximum stack of " + burnEffect.stackTime);
            return;
        }
        Debug.Log("Burn Damage : " + burnEffect.damagePerRate.value + " Rate " + burnEffect.rate.value);
        tempDamage = burnEffect.damagePerRate.value;
        tempRate = burnEffect.rate.value;
        timer = tempRate;
        if (effectStacks < burnEffect.stackTime && effect.effectType.HasFlag(EffectType.StackingEffect))
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
    }
    public override void End()
    {
        BurnEffect burnEffect = (BurnEffect)effect;
        burnEffect.damagePerRate.RemoveAllModifiersFromSource(this);
        _enemy.EffectColor(Color.white);
        //EffectStacks = 0;
    }
    public override StringBuilder Display()
    {
        sb.Clear();
        sb.Append("Effect ID :" + effect.ID + "\n");
        sb.Append("SumDamage :" + sumDamage + "\n");
        sb.Append("Damage :" + tempDamage + "\n");
        sb.Append("Rate :" + tempRate + "\n");
        sb.Append("Duration :" + _duration + "\n");
        sb.Append("Timer :" + timer + "\n");
        sb.Append("Description :" + effect.description + "\n");
        sb.Append("/////////////////////////////////////////////////////////\n");
        return sb;
    }
}
