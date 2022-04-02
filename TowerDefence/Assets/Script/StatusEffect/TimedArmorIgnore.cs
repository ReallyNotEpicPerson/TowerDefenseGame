using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedArmorIgnore : TimedEffect
{
    private readonly Enemy _enemy;

    public TimedArmorIgnore(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }

    public override void End()
    {
        throw new System.NotImplementedException();
    }

    protected override void ApplyEffect()
    {
        throw new System.NotImplementedException();
    }
}
