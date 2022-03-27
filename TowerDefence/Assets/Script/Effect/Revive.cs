using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Revive", menuName = "Revive")]
public class Revive : BaseEffect
{
    public override TimedEffect init(GameObject obj)
    {
        return new TimedReviveEffect(this, obj);
    }

}
