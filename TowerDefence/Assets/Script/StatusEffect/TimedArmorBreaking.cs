using UnityEngine;

public class TimedArmorBreaking : TimedEffect
{
    private readonly Enemy _enemy;
    private bool haveArmorAtStart = false;
    public TimedArmorBreaking(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void End()
    {
        if (haveArmorAtStart || _enemy.armorStat.HaveArmor())
        {
            _enemy.EnableState(EnemyState.Armored);
        }
        _enemy.RemoveFX(Effect.ID);
    }

    public override void Tick()
    {
        if (_enemy.enemyState.HasFlag(EnemyState.Armored) && haveArmorAtStart == false)
        {
            _enemy.DisableState(EnemyState.Armored);
        }
        if (Effect.expirableType.HasFlag(ExpirableType.NonExpireable))
        {
            return;
        }
        else if (Effect.expirableType.HasFlag(ExpirableType.Expirable))
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0)
            {
                End();
                IsFinished = true;
            }
        }
    }
    protected override void ApplyEffect()
    {
        if (_enemy.enemyState.HasFlag(EnemyState.Armored))
        {
            haveArmorAtStart = true;
            _enemy.DisableState(EnemyState.Armored);
        }
        else
        {
            haveArmorAtStart = false;
            Debug.Log("no armor,man!");
            return;
        }
        if (!_enemy.ContainFX(Effect.ID))
        {
            _enemy.AddFX(this);
        }
    }
}
