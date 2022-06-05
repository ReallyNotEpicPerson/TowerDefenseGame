using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour//use as an epic manager for a turret
{
    [SerializeField] List<BaseEffect> listOfDebuffs;

    public bool HaveStatusEffect()
    {
        if (listOfDebuffs.Count == 0)
        {
            Debug.LogError("You forgot sth");
            return false;
        }
        return true;
    }
    public BaseEffect GetSpeedBoostData()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is SlowEffect && listOfDebuffs[i].ID.Contains("TUR"))
            {
                return listOfDebuffs[i];
            }
        }
        Debug.LogError("forget to add eii?");
        return null;
    }
    public bool HaveThis(BaseEffect bfx)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] == bfx)
            {
                return true;
            }
        }
        return false;
    }
    public void Insta_kill(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Insta_Kill)
            {
                if (listOfDebuffs[i].CanDie())
                {
                    enemy.When_Insta_kill();
                    break;
                }
            }
        }
    }
    public void Cleanse(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Cleanse)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.RemoveALLDebuff();
                }
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public bool Revive(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Revive && !enemy.HaveThis(listOfDebuffs[i].ID))
            {
                Revive rev = listOfDebuffs[i] as Revive;
                if (Random.value <= listOfDebuffs[i].chance && enemy.GetReviveTime() < rev.reviveTime)
                {
                    Debug.Log("He live this time");
                    enemy.AddDebuff(listOfDebuffs[i]);
                    return true;
                }
            }
        }
        return false;
    }
    public void Slow(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is SlowEffect)
            {
                if (listOfDebuffs[i].ID.Contains("TUR"))
                {
                    continue;
                }
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
            }
        }
        //Debug.LogError("nothing");
    }
    public void Dots(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is DotsEffect)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    if (enemy.CheckEnemyType(EnemyType.ImmuneToFire) && listOfDebuffs[i].ID.Contains("BU"))
                    {
                        continue;
                    }
                    if (enemy.CheckEnemyType(EnemyType.ImmuneToPoison) && listOfDebuffs[i].ID.Contains("POI"))
                    {
                        continue;
                    }
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
            }
        }
        //Debug.LogError("nothing");
    }
    public void Fear(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is FearEffect)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public void Weaken(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Weaken)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public void Invisible(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Invisible)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public void DisableArmor(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is ArmorBreaking)
            {
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                }
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public BaseEffect GetDamageBoostData()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Weaken && listOfDebuffs[i].ID.Contains("TUR"))
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("forget to add eii?");
        return null;
    }
    public BaseEffect GetReviveEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Revive)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetDOTSEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is DotsEffect)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetFearEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is FearEffect)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetWeakenEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Weaken)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetInvisibleEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Invisible)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetArmorBreakEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is ArmorBreaking)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetSlowEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is SlowEffect)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
    public BaseEffect GetInsta_KillEffect()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Insta_Kill)
            {
                return listOfDebuffs[i];
            }
            continue;
        }
        Debug.LogError("Oh....Forgot sth?");
        return null;
    }
}
