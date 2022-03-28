using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour//use as an epic manager for a turret
{
    [SerializeField] List<BaseEffect> listOfDebuffs;

    public List<BaseEffect> GetList()
    {
        return listOfDebuffs;
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
                Insta_Kill insta_kill = (Insta_Kill)listOfDebuffs[i];
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
                if (listOfDebuffs[i] is SlowEffect && Random.value <= listOfDebuffs[i].chance)
                {
                    enemy.RemoveALLDebuff();
                }

            }
        }
        Debug.LogError("nothing");
    }
    public bool Revive(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (Random.value <= listOfDebuffs[i].chance && listOfDebuffs[i] is Revive && !enemy.HaveThis(listOfDebuffs[i].ID))
            {
                enemy.AddDebuff(listOfDebuffs[i]);
                return true;
            }
        }
        return false;
    }
    public void Slow(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if(listOfDebuffs[i] is SlowEffect && Random.value <= listOfDebuffs[i].chance)
            {
                enemy.AddDebuff(listOfDebuffs[i]);
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public void Burn(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is BurnEffect && Random.value <= listOfDebuffs[i].chance)
            {
                enemy.AddDebuff(listOfDebuffs[i]);
                return;
            }
        }
        Debug.LogError("nothing");
    }
    public void Heal()
    {

    }
    public void Fear(Enemy enemy)
    {
        for (int i = 0; i < listOfDebuffs.Count; i++)
        {
            if (listOfDebuffs[i] is FearEffect && Random.value <= listOfDebuffs[i].chance)
            {
                enemy.AddDebuff(listOfDebuffs[i]);
                return;
            }
        }
        Debug.LogError("nothing");
    }
}
