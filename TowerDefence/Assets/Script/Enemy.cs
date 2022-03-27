using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region ENUM
[System.Flags]
public enum EnemyState
{
    None = 0, Slow = 1 << 0, DOTS = 1 << 1, Amored = 1 << 2, Insta_Killed = 1 << 3, Fear = 1 << 4,
    //BlueFlamed = Slow | DOTS,
    //ArmorBurn = Slow | Amored,
    //Slow2=1<<2,?
    //Slow3=1<<3,?
}
[System.Flags]
public enum EnemyType
{
    None = 0, Fast = 1 << 0,
    Tough = 1 << 1,
    Armored = 1 << 2,
    Special = 1 << 3,
    //Immunity = 1 << 4,
    //ImmunityToAll = 1 << 5,
}
public enum movementType
{
    FixedPath,
    NavMesh,
}
#endregion 
public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyType enemyType;
    [SerializeField] public EnemyState enemyState;
    #region EnemyStat
    public float startSpeed = 10f;
    public float startHealth = 100f;
    public Armor armorStat;

    public bool UsePathFinding;
    private NavMeshAI enemyNavMeshMovement;
    private EnemyMovement enemyPathMovement;
    //[HideInInspector]
    public float speed;
    private float health;
    public int worth = 20;
    #endregion

    public GameObject dedFX;
    GameObject Ded;

    [SerializeField] private Image healthBar;
    private bool isDead = false;
    private SpriteRenderer enemyColor;
    [SerializeField] private GameObject Displayer;

    [SerializeField] private EntityEffectHandler Handler;
    [SerializeField] private EffectManager fxManager;

    StatUI_InGame statUI;
    #region wait for second List
    static Dictionary<int, WaitForSeconds> dictWaitForSecond = new Dictionary<int, WaitForSeconds>();
    const float FloatToIntRate = 1000;
    public static WaitForSeconds WaitFor(int seconds)
    {
        WaitForSeconds wfs;
        if (!dictWaitForSecond.TryGetValue(seconds, out wfs))
        {
            dictWaitForSecond.Add(seconds, wfs = new WaitForSeconds((float)seconds / FloatToIntRate));
            Debug.Log(seconds + " " + (float)seconds / FloatToIntRate);
        }
        return wfs;
    }
    #endregion
    #region run in the editor
    private void OnValidate()
    {
        if (healthBar == null)//change
        {
            healthBar = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        }
        enemyColor = GetComponent<SpriteRenderer>();
        if (!TryGetComponent(out armorStat))
        {
            //Debug.LogError("you forgot the armor bro");
            return;
        }
        if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
        {
            EnableState(EnemyState.Amored);
        }
        else if (armorStat.armorType.HasFlag(ArmorType.None))
        {
            DisableState(EnemyState.Amored);
        }
        if (UsePathFinding)
        {
            enemyNavMeshMovement = GetComponent<NavMeshAI>();
        }
        //TryGetComponent(out fxManager);
        
    }
    #endregion

    private void Awake()
    {
        enemyColor = GetComponent<SpriteRenderer>();
        if (!TryGetComponent(out armorStat))
        {
            //Debug.LogError("you forgot the armor bro");
            return;
        }
        if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
        {
            EnableState(EnemyState.Amored);
        }
        else if (armorStat.armorType.HasFlag(ArmorType.None))
        {
            DisableState(EnemyState.Amored);
        }
        if (UsePathFinding)
        {
            enemyNavMeshMovement = GetComponent<NavMeshAI>();
        }
        else if (!UsePathFinding)
        {
            enemyPathMovement = GetComponent<EnemyMovement>();
        }
        TryGetComponent(out fxManager);
    }
    void Start()
    {
        speed = startSpeed;
        health = startHealth;
        Ded = Instantiate(dedFX, transform.position, Quaternion.identity);
        Ded.SetActive(false);
    }
    #region Damaging
    public void TakeDamage(float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        if (enemyState.HasFlag(EnemyState.Amored))
        {
            if (armorStat.DamageArmor(amount) == false)
            {
                DisableState(EnemyState.Amored);
            }
            return;
        }
        switch (type)
        {
            case DamageDisplayerType.Normal:
                DamageDisplayer.Create(transform.position, amount);
                break;
            case DamageDisplayerType.Critial:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.Critial);
                break;
            case DamageDisplayerType.Burned:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.Burned);
                break;
            case DamageDisplayerType.ArmorPenetration:
                break;
            default:
                Debug.LogError("But How?");
                break;
        }
        health -= amount;
        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            if (fxManager != null)
            {
                fxManager.Revive(this);
                return;
            }
            Die();
        }
    }
    public void ArmorPiercing(float amount)
    {
        health -= amount;
        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            if (fxManager != null)
            {
                fxManager.Revive(this);
                return;
            }
            Die();
        }
    }
    #endregion
    public bool IsDed()//idk
    {
        return isDead;
    }
    #region Handler
    public void AddDebuff(BaseEffect baseEffect)//use this instead,like EVERY TIME
    {
        Handler.AddDebuff(baseEffect, this);
    }
    public bool HaveThis(string ID)
    {
        return Handler.contain(ID);
    }
    public void RemoveALLDebuff()
    {
        Handler.RemoveALLDebuff();
    }
    #endregion
    void Die()
    {
        isDead = true;
        PlayerStat.moneyInGame += worth;
        Ded.SetActive(true);
        //WaitFor((int)(2*FloatToIntRate));
        TheSpawner.numOfEnemies--;
        Ded.SetActive(false);
        gameObject.SetActive(false);
    }
    public void FakeDeath()
    {        
        Debug.Log("Sleep");
        health = startHealth;
        armorStat.restoreArmor();
        //gameObject.SetActive(false);
        //GetComponent<EntityEffectHandler>().enabled=true;
        //throw new NotImplementedException();
    }
    public void Revive()
    {
        Debug.Log("wake");
        //gameObject.SetActive(true);
        //throw new NotImplementedException();
    }
    #region [depricated]
    public void EnemyType(out EnemyType type)//get rid of as soon as possible
    {
        type = enemyType;
    }
    #endregion

    #region for adjusting speed
    public void TurnBack(int i)
    {
        if (UsePathFinding)
        {
            //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
            enemyNavMeshMovement.TurnBack(i);
        }
        else // call enemy movement component instead 
        {
            //enemyPathMovement.AdjustSpeed(slowPtc);
            //speed = startSpeed * (1 - slowPtc);
        }
    }
    public void IncreaseSpeed(StatModifier mod)
    {
        EnableState(EnemyState.Fear);
        if (UsePathFinding)
        {
            //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
            enemyNavMeshMovement.AddSpeedMod(mod);
        }
        else // call enemy movement component instead 
        {
            //enemyPathMovement.AdjustSpeed(slowPtc);
            //speed = startSpeed * (1 - slowPtc);
        }
    }
    public void SlowDown(StatModifier mod)
    {
        EnableState(EnemyState.Slow);
        EffectColor(Color.blue);
        if (UsePathFinding)
        {
            //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
            enemyNavMeshMovement.AddSpeedMod(mod);
        }
        else // call enemy movement component instead 
        {
            //enemyPathMovement.AdjustSpeed(slowPtc);
            //speed = startSpeed * (1 - slowPtc);
        }
        //Debug.Log("Work!,GOD DAMMIT");
    }
    public void ModifyAcceleration(StatModifier mod)
    {
        //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
        enemyNavMeshMovement.AddSpeedMod(mod);
    }
    public void UndoModification(object source)
    {
        DisableState(EnemyState.Slow);
        if (UsePathFinding)
        {
            enemyNavMeshMovement.RemoveMod(source);
        }
        else
        {
            //speed = startSpeed;
        }
        EffectColor(Color.white);
    }
    #endregion
    /*
    public void ConstantSlow(float ptc)
    {
        if (1 - ptc > ListOfMOD[1])
        {
            speed = startSpeed * (1 - ptc);
            ListOfMOD[1] = (1 - ptc);
        }
        ListOfMOD[0] += 0.1f;
        if (!enemyState.HasFlag(EnemyState.Slow))
        {
            EnableState(EnemyState.Slow);
        }

        /*if (gameObject.activeSelf == true)
        {
            StartCoroutine(SlowPerSecondCoroutine(ptc, 0.2f));
        }
    }fix this crap*/

    public void When_Insta_kill()//remove soon??
    {
        GameObject sth = Instantiate(Displayer, transform.position, Quaternion.identity);
        sth.transform.GetChild(0).GetComponentInChildren<TMP_TextController>().SetText("Insta Kill!!!");
        Destroy(sth, 0.5f);
        Handler.RemoveALLDebuff();
        Die();//pooling
    }
    public void EffectColor(Color color)
    {
        enemyColor.color = color;
    }
    void OnMouseEnter()
    {
        //statUI.TransferCharacter(gameObject);
        //statUI.enabled = true;
    }
    void OnMouseExit()
    {
        //statUI.enabled = false;
    }
    #region flag checking/Manipulation
    public bool CheckState(EnemyState es)
    {
        return enemyState.HasFlag(es);
    }
    public void DisableState(EnemyState es)
    {
        enemyState &= ~es;
    }
    public void EnableState(EnemyState es)
    {
        enemyState |= es;
    }
    #endregion

}
