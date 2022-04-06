using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedInvisible : TimedEffect
{
    private readonly Enemy _enemy;

    public TimedInvisible(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }

    public override void End()
    {
        _enemy.ReverseBlackDad();
        //_enemy.RemoveFX(Effect.ID);
    }

    protected override void ApplyEffect()
    {
        _enemy.Invisible();
        //_enemy.AddFX(this);
    }
}
