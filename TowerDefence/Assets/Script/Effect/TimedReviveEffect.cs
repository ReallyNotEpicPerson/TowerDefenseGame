﻿using System.Collections;
using UnityEngine;

public class TimedReviveEffect : TimedEffect
{
    private readonly Enemy _enemy;

    public TimedReviveEffect(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }

    public override void End()
    {
        _enemy.Revive();
        _enemy.gameObject.SetActive(true);
    }

    protected override void ApplyEffect()
    {
        Debug.Log("activate");
        if (effect.effectType.HasFlag(EffectType.LateActivation))
        {
            Debug.Log("fake own death");
            _enemy.FakeDeath();            
            return;
        }
    }
}