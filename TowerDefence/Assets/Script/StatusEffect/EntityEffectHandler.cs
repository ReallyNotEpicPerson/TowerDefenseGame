using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityEffectHandler : MonoBehaviour
{
    public Dictionary<string, TimedEffect> _effectList = new Dictionary<string, TimedEffect>();

    #region Update Effect
    public void Update()
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
                RemoveDebuff(effect.Effect.ID);
                ListAllDebuff();
            }
        }
    }
    #endregion
    public bool Contain(string ID)
    {
        return _effectList.ContainsKey(ID);
    }
    public void ActivateDebuff(TimedEffect timedEffect)
    {

        if (_effectList.ContainsKey(timedEffect.Effect.ID))
        {
            _effectList[timedEffect.Effect.ID].Activate();
        }
        else
        {
            _effectList.Add(timedEffect.Effect.ID, timedEffect);
            _effectList[timedEffect.Effect.ID].Activate();

        }
        ListAllDebuff();
    }

    public void AddDebuff(BaseEffect baseEffect, GameObject Obj)// the ultimate function
    {
        /*switch (baseEffect)
        {
            case SlowEffect slowEffect:
                ActivateDebuff(slowEffect.Init(enemy.gameObject));
                break;
            case BurnEffect burnEffect:
                ActivateDebuff(burnEffect.Init(enemy.gameObject));
                break;
            case FearEffect fearEffect:
                ActivateDebuff(fearEffect.Init(enemy.gameObject));
                break;
            case Revive revive:
                ActivateDebuff(revive.Init(enemy.gameObject));
                break;
            case Weaken weaken:
                ActivateDebuff(weaken.Init(enemy.gameObject));
                break;
            case DisableArmor disableArmor:
                ActivateDebuff(disableArmor.Init(enemy.gameObject));
                break;
            default:
                Debug.LogError("wait what the fuck is this?");
                break;
        }*/
        switch (baseEffect)
        {
            case SlowEffect slowEffect:
                if (_effectList.ContainsKey(slowEffect.ID))
                {
                    _effectList[slowEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(slowEffect.ID, slowEffect.Init(Obj));
                    _effectList[slowEffect.ID].Activate();
                }
                break;
            case DotsEffect dotsEffect:
                if (_effectList.ContainsKey(dotsEffect.ID))
                {
                    _effectList[dotsEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(dotsEffect.ID, dotsEffect.Init(Obj));
                    _effectList[dotsEffect.ID].Activate();
                }
                break;
            case FearEffect fearEffect:
                if (_effectList.ContainsKey(fearEffect.ID))
                {
                    _effectList[fearEffect.ID].Activate();
                }
                else
                {
                    _effectList.Add(fearEffect.ID, fearEffect.Init(Obj));
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
                    _effectList.Add(revive.ID, revive.Init(Obj));
                    _effectList[revive.ID].Activate();
                }
                break;
            case Weaken weaken:
                if (_effectList.ContainsKey(weaken.ID))
                {
                    _effectList[weaken.ID].Activate();
                }
                else
                {
                    _effectList.Add(weaken.ID, weaken.Init(Obj));
                    _effectList[weaken.ID].Activate();
                }
                break;
            case DisableArmor disableArmor:
                if (_effectList.ContainsKey(disableArmor.ID))
                {
                    _effectList[disableArmor.ID].Activate();
                }
                else
                {
                    _effectList.Add(disableArmor.ID, disableArmor.Init(Obj));
                    _effectList[disableArmor.ID].Activate();
                }
                break;
            case Invisible invisible:
                if (_effectList.ContainsKey(invisible.ID))
                {
                    _effectList[invisible.ID].Activate();
                }
                else
                {
                    _effectList.Add(invisible.ID, invisible.Init(Obj));
                    _effectList[invisible.ID].Activate();
                }
                break;
            default:
                Debug.LogError("wait what the fuck is this?");
                break;
        }
    }

    public void RemoveDebuff(string ID)//OG remove
    {
        _effectList.Remove(ID);

    }
    /*public void RemoveALLDebuff(int LeType) //Well fuck,there are 2 type of status effect negative=0 and positive=1
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
    }*/
    public void RemoveAllDebuffExcept()//string name="Revive")
    {
        foreach (KeyValuePair<string, TimedEffect> effect in _effectList)
        {
            if (effect.Value is TimedReviveEffect)
            {
                continue;
            }
            _effectList.Remove(effect.Key);
        }
    }
    public void RemoveALLDebuff()
    {
        _effectList.Clear();
        Debug.Log("No Debuff left right? " + _effectList.Count());
        if (_effectList.Count > 0)
        {
            Debug.Log("WTF");
        }
    }

    public void ListAllDebuff()
    {
        //Debug.Log(_effectList.Count());
        foreach (KeyValuePair<string, TimedEffect> pair in _effectList)
        {
            Debug.Log(pair + "\n");
        }
    }
}