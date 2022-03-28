using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityEffectHandler : MonoBehaviour
{
    public Dictionary<string, TimedEffect> _effectList = new Dictionary<string, TimedEffect>();

    #region Update Effect
    void Update()
    {
        HandleDebuff();
    }
    public void HandleDebuff()
    {
        if (_effectList.Count == 0)
        {
            return;
        }
        foreach (var effect in _effectList.Values.ToList())
        {
            effect.Tick();
            if (effect.IsFinished)
            {
                RemoveDebuff(effect.effect.ID);
                ListAllDebuff();
            }
        }
    }
    #endregion
    public bool contain(string ID)
    {
        return _effectList.ContainsKey(ID);
    }
    public void AddDebuff(BaseEffect baseEffect, Enemy enemy)// the ultimate function
    {
        switch (baseEffect)
        {
            case SlowEffect slowEffect:
                if (_effectList.ContainsKey(slowEffect.ID))
                {
                    _effectList[slowEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(slowEffect.ID, slowEffect.init(enemy.gameObject));
                    _effectList[slowEffect.ID].Activate();
                }
                break;
            case BurnEffect burnEffect:
                if (_effectList.ContainsKey(burnEffect.ID))
                {
                    _effectList[burnEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(burnEffect.ID, burnEffect.init(enemy.gameObject));
                    _effectList[burnEffect.ID].Activate();
                }
                break;
            case HealEffect healEffect:
                Debug.Log(healEffect.ID);
                break;
            case FearEffect fearEffect:
                if (_effectList.ContainsKey(fearEffect.ID))
                {
                    _effectList[fearEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(fearEffect.ID, fearEffect.init(enemy.gameObject));
                    _effectList[fearEffect.ID].Activate();
                }
                break;
            case Revive revive:
                if (_effectList.ContainsKey(revive.ID))
                {
                    _effectList[revive.ID].Activate();
                }
                else
                {
                    _effectList.Add(revive.ID, revive.init(enemy.gameObject));
                    _effectList[revive.ID].Activate();
                }
                break;
            default:
                Debug.LogError("wait what the fuck is this?");
                break;
        }
        ListAllDebuff();
    }
    public void RemoveDebuff(string ID)//OG remove
    {
        _effectList.Remove(ID);
    }
    public void RemoveALLDebuff(int LeType) //Well fuck,there are 2 type of status effect negative=0 and positive=1
    {
        if (LeType == 0)
        {
            foreach (var effect in _effectList.Values.ToList())
            {
                //remove all negative effect somehow 
            }
        }
        if (LeType == 0)
        {
            foreach (var effect in _effectList.Values.ToList())
            {
                //remove all positive effect somehow 
            }
        }
    }
    public void RemoveAllDebuffExcept()
    {

    }
    public void RemoveALLDebuff()
    {
        _effectList.Clear();
        Debug.Log("No Debuff left right? "+_effectList.Count());
    }
    
    public void ListAllDebuff()
    {
        //Debug.Log(_effectList.Count());
        foreach (KeyValuePair<string, TimedEffect> pair in _effectList)
        {
            Debug.Log( pair +"\n");
        }
    }
}