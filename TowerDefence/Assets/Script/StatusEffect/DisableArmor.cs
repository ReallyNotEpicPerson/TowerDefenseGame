using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisableArmor", menuName = "DisableArmor")]
public class DisableArmor : BaseEffect
{
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedDisableArmor(this, obj);
    }
}
