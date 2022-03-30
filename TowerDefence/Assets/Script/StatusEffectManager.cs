using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour//, IDamageable<float,float,float>
{
    [SerializeField] private List<Enemy> enemyList;
    [SerializeField] float[] ListOfMOD;//gone?keep but change shit
    #region wait for second List
    private const float FloatToIntRate = 1000;
    static Dictionary<int, WaitForSeconds> dictWaitForSecond = new Dictionary<int, WaitForSeconds>();
    public static WaitForSeconds WaitFor(int seconds)
    {
        
        if (!dictWaitForSecond.TryGetValue(seconds, out WaitForSeconds wfs))
        {
            dictWaitForSecond.Add(seconds, wfs = new WaitForSeconds((float)seconds / FloatToIntRate));
            Debug.Log(seconds + " " + (float)seconds / FloatToIntRate);
        }
        return wfs;
    }
    #endregion
    public List<int> TickTimer;
    //Dictionary<List<string>, int> Adict;
    void Start()
    {
        //Adict = new Dictionary<List<string>, int>();
        ListOfMOD = new float[20];
    }
    public void Update()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (!enemyList[i].isActiveAndEnabled)
            {
                enemyList.RemoveAt(i);
            }
            if (enemyList[i].CheckState(EnemyState.Slow))
            {
                if (ListOfMOD[0] < 0)
                {
                    enemyList[i].DisableState(EnemyState.Slow);
                    enemyList[i].speed = enemyList[i].startSpeed;
                    enemyList[i].EffectColor(Color.white);
                    ListOfMOD[1] = 0;
                    return;
                }
                ListOfMOD[0] = ListOfMOD[0] - Time.deltaTime;
                enemyList[i].EffectColor(Color.blue);
            }
        }
    }
    public void BurnEnemy(int ticks)//rework
    {
        if (TickTimer.Count <= 0)
        {
            TickTimer.Add(ticks);
        }
        else
        {

        }
    }
    public void SlowDownEnemy(int i,float ptc, float sec = 0.1f)// rework
    {
        if (1 - ptc > ListOfMOD[1])
        {
            enemyList[i].speed = enemyList[i].startSpeed * (1 - ptc);
            ListOfMOD[1] = (1 - ptc);
        }
        //if(damageType.HasFlag(DamageType.StackableDuration))
            //ListOfMOD[0] += sec;
        else
            ListOfMOD[0] = sec;
        if (!enemyList[i].CheckState(EnemyState.Slow))
        {
            enemyList[i].EnableState(EnemyState.Slow);
        }
    }
    public void AddEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }
}
