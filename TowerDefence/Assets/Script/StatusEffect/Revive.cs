using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Revive", menuName = "Revive")]
public class Revive : BaseEffect
{
    public int reviveTime = 1;
    public override void OnValidate()
    {
        base.OnValidate();
        if (!effectType.HasFlag(EffectType.LateActivation))
        {
            Debug.LogError("only LateActivation....sorry");
        }
    }
    public override TimedEffect Init(GameObject obj)
    {
        return new TimedReviveEffect(this, obj);
    }

}
