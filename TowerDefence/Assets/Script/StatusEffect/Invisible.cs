using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invisible Effect", menuName = "Invisible")]
public class Invisible : BaseEffect
{
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedInvisible(this,obj);
    }
}
