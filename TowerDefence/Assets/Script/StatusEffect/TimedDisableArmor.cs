using UnityEngine;

public class TimedDisableArmor : TimedEffect
{
    private readonly Enemy _enemy;
    private bool haveArmorAtStart = false;
    public TimedDisableArmor(BaseEffect buff, GameObject obj) : base(buff, obj)
    {
        _enemy = obj.GetComponent<Enemy>();
    }
    public override void End()
    {
        if (haveArmorAtStart || _enemy.armorStat.HaveArmor())
        {
            _enemy.EnableState(EnemyState.Amored);
        }
        //_enemy.RemoveFX(Effect.ID);
    }

    public override void Tick()
    {
        if (_enemy.enemyState.HasFlag(EnemyState.Amored) && haveArmorAtStart == false)
        {
            _enemy.DisableState(EnemyState.Amored);
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
        if (_enemy.enemyState.HasFlag(EnemyState.Amored))
        {
            haveArmorAtStart = true;
            _enemy.DisableState(EnemyState.Amored);
            //_enemy.AddFX(this);
        }
        else
        {
            haveArmorAtStart = false;
            //_enemy.AddFX(this);
            Debug.Log("no armor,man!");
            return;
        }
    }
}
