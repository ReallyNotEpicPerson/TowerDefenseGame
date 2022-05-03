using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStat
{
    public float baseValue;
    protected bool isDirty = true;
    protected float lastBaseValue = float.MinValue;
    protected float _value;

    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public float Value
    {
        get
        {
            if (isDirty || lastBaseValue != baseValue)
            {
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }
    public CharacterStat(float BaseValue) : this()
    {
        baseValue = BaseValue;
    }
    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
    public virtual float value { get { return CalculateFinalValue(); } }

    public void AddingOneInstance(StatModifier mod)
    {
        if (RemoveModifier(mod.source))
        {
            AddModifier(mod);
            //Debug.Log("Yes,There is a version of this "+value);
        }
        else
        {
            AddModifier(mod);
            //Debug.Log("NO,So fuck it "+ value);
        }
    }
    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        //Debug.Log("number of statmod " + statModifiers.Count);
        statModifiers.Sort(CompareModifierOrder);
    }
    public virtual bool RemoveModifier(object source)
    {
        bool didRemove = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
                break;
            }
        }
        return didRemove;
    }
    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }
    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers
        float preDebuff = baseValue;
        float BestDebuff = float.PositiveInfinity;
        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            //Debug.Log(finalValue);
            if (mod.type == StatModType.Flat)
            {
                finalValue += mod.value;
                //Debug.Log("flat");
            }
            else if (mod.type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
            {
                sumPercentAdd += mod.value; // Start adding together all modifiers of this type

                // If we're at the end of the list OR the next modifer isn't of this type
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.PercentAdd)
                {
                    finalValue *= (1 + sumPercentAdd); // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                    sumPercentAdd = 0; // Reset the sum back to 0
                }
                //Debug.Log("% add");
            }
            else if (mod.type == StatModType.PercentMult) // Percent renamed to PercentMult
            {
                finalValue *= (1 + mod.value);
                //Debug.Log("Statcount:"+statModifiers.Count+" value : " + mod.value + " % mul : " + finalValue);
            }
            else if (mod.type == StatModType.PercentDebuffBest)
            {
                //Debug.Log("% debuff");
                if (i == 0 || (i - 1 >= 0 && statModifiers[i - 1].type != StatModType.PercentDebuffBest))
                {
                    preDebuff = finalValue;
                    //Debug.Log("Pre Debuff value " + preDebuff + "\n" + "Number Of Mod " + statModifiers.Count);
                }
                if (BestDebuff > preDebuff * (1 - mod.value))
                {
                    BestDebuff = preDebuff * (1 - mod.value);
                    //Debug.Log(BestDebuff);
                    finalValue = preDebuff * (1 - mod.value);
                }
                else
                {
                    continue;
                }
            }
        }
        // Rounding gets around dumb float calculation errors (like getting 12.0001f, instead of 12f)
        // 4 significant digits is usually precise enough, but feel free to change this to fit your needs
        return (float)Math.Round(finalValue, 4);
    }
    public bool HaveSameType(object source)
    {
        for (int i = 0; i < statModifiers.Count; i++)
        {
            if (statModifiers[i].source.GetType() == source.GetType())
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveAllModifier()
    {
        statModifiers.Clear();
    }
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        //Debug.Log("number of statmod left:" + statModifiers.Count);
        return didRemove;
    }
}
