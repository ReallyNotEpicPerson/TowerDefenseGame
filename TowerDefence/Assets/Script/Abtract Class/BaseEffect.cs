using UnityEngine;

[System.Flags]
public enum EffectType
{
    None = 0,
    StackingEffect = 1 << 1,//this effect 
    StackableDuration = 1 << 2,
    LateActivation = 1 << 3,//?when??
}
public enum RemovableType { Removable, NonRemovable };
//Removable - type : can be remove by being expired || can be remove by using other mean
//NonRemovable - type : can be remove by being expired && cannot be remove by using other mean
public enum ExpirableType { Expirable, NonExpireable };
//Expirable : duration reduce over time , can extend duration
//NonExpirable : duration never drop , extending duration have no meaning
public abstract class BaseEffect : ScriptableObject
{
    public string ID;
    public Sprite sprite;
    public EffectType effectType;
    public RemovableType removableType;
    public ExpirableType expirableType;
    protected bool activate = false;
    public float _duration = 0.0f;
    public float chance=0;

    [TextArea(3, 6)]
    public string description;

    public virtual void OnValidate()
    {
        if (expirableType.HasFlag(ExpirableType.NonExpireable))
        {
            effectType &= ~EffectType.LateActivation;
            //DisableState(EffectType.LateActivation);
        }/*
        else if (expirableType.HasFlag(ExpirableType.Expirable))
        {

        }
        else if (removableType.HasFlag(RemovableType.NonRemovable))
        {

        }
        else if (removableType.HasFlag(RemovableType.Removable))
        {

        }*/
        if(chance > 1 || chance < 0)
        {
            chance = 1;
        }
    }
    public abstract TimedEffect Init(GameObject obj);
    #region state control
    public void DisableState(EffectType et)
    {
        effectType &= ~et;
    }
    public void EnableState(EffectType et)
    {
        effectType |= et;
    }
    #endregion
}