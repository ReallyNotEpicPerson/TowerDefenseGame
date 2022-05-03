using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisableArmor", menuName = "DisableArmor")]
public class ArmorBreaking : BaseEffect
{
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedArmorBreaking(this, obj);
    }
}
