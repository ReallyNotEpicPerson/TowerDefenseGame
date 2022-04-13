using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperClass
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
}
