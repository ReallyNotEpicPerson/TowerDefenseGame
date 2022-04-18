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
    }/*
    public bool HaveStatusEffectSlow(BaseEffect Bfx ,out BaseEffect fx)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if(listOfDebuffs[i] is SlowEffect && Bfx is SlowEffect)
            {
                
                return true;
            }
        }
        if (listOfDebuffs.Count == 0)
        {
            Debug.LogError("You forgot sth");
            out fx;
            return false;
        }
        //return false;
    }*/
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
    public BaseEffect GetDamageBoostData()
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is Weaken && listOfDebuffs[i].ID.Contains("TUR"))
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
                Insta_Kill insta_kill = listOfDebuffs[i] as Insta_Kill;
                if (insta_kill.CanKYS())
                {
                    enemy.When_Insta_kill();
                }
            }
        }
        Debug.LogError("nothing");
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
                if (Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.AddDebuff(listOfDebuffs[i]);
                    return true;
                }
            }
        }
        Debug.LogError("Forgot them data man");
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
            if (listOfDebuffs[i] is DisableArmor)
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

}
