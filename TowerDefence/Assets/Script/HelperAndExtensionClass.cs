using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class HelperAndExtensionClass
{
    static readonly Dictionary<int, WaitForSeconds> dictWaitForSecond = new Dictionary<int, WaitForSeconds>();
    const float FloatToIntRate = 1000;
    public static WaitForSeconds WaitFor(int seconds)
    {
        if (!dictWaitForSecond.TryGetValue(seconds, out WaitForSeconds wfs))
        {
            dictWaitForSecond.Add(seconds, wfs = new WaitForSeconds((float)seconds / FloatToIntRate));
            Debug.Log(seconds + " " + (float)seconds / FloatToIntRate);
        }
        return wfs;
    }
    public static void AddSpecialAbility(this EntityEffectHandler handler,BaseEffect fx,GameObject target)
    {
        handler.AddDebuff(fx,target);
    }
    public static void shit(this EffectManager fxManager)
    {
        return;
    }
    public static Vector3 RandomPointInCircle(float range)
    {
        return Random.insideUnitCircle * Random.Range(-range, range);
    }
    public static void Fade(this SpriteRenderer spr,float ptc)
    {
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b,ptc);
    }
    
}
