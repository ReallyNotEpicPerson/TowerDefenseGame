using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region ENUM
[System.Flags]
public enum EnemyState
{
    None = 0, Slow = 1 << 1, Weaken = 1 << 2, Amored = 1 << 3, TakeNoDamage = 1 << 4, Fear = 1 << 5,
    //BlueFlamed = Slow | DOTS,
    //ArmorBurn = Slow | Amored,
    //Slow2=1<<2,?
    //Slow3=1<<3,?
}
[System.Flags]
public enum EnemyType
{
    None = 0,//just a normal enemy
    Mutant = 1 << 1,// have one or more special ability
    ImmunityToAll = 1 << 3,//immune to all status effect,even the good one?
}
public enum MovementType
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
    private StatValueType modifier; 
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
    static readonly Dictionary<int, WaitForSeconds> dictWaitForSecond = new Dictionary<int, WaitForSeconds>();
    const float FloatToIntRate = 1000;
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
    #region run in the editor
    private void OnValidate()
    {
        if (healthBar == null)//change
        {
            healthBar = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        }
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
        if (enemyType.HasFlag(EnemyType.Mutant))
        {
            fxManager.HaveStatusEffect();
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
    public bool TakeDamage(float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        if (enemyState.HasFlag(EnemyState.TakeNoDamage))
        {
            return false;
        }
        if (enemyState.HasFlag(EnemyState.Amored))
        {
            if (armorStat.DamageArmor(amount) == false)
            {
                DisableState(EnemyState.Amored);
            }
            return true;
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
        if (health > startHealth)
        {
            health = startHealth;
        }
        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            if (fxManager != null && fxManager.Revive(this))
            {
                Debug.Log("He live this time");
                return false;
            }
            Debug.Log("Nope .... he died");
            Die();
        }
        return true;
    }
    public void ArmorPiercing(float amount)
    {
        health -= amount;
        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            if (fxManager != null && fxManager.Revive(this))
            {
                Debug.Log("He wont die this time");
                return;
            }
            Debug.Log("He ded , oopsie daisy");
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
        return Handler.Contain(ID);
    }
    public void RemoveALLDebuff()
    {
        Handler.RemoveALLDebuff();
    }
    public void RemoveALLDebuffExcept()
    {
        Handler.RemoveAllDebuffExcept();
    }
    #endregion
    #region EnemyDeaths
    void Die()
    {
        RemoveALLDebuff();
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
        gameObject.tag = "Untagged";
        if (enemyNavMeshMovement != null)
        {
            GetComponent<NavMeshAI>().Enable(false);
        }
        else if (enemyPathMovement != null)
        {
            GetComponent<EnemyMovement>().enabled = false;
        }
        enemyColor.enabled = false;
        EnableState(EnemyState.TakeNoDamage);
        //GetComponent<EntityEffectHandler>().enabled=true;
        //throw new NotImplementedException();
    }
    public void Revive()
    {
        Debug.Log("wake");
        RemoveALLDebuff();
        DisableState(EnemyState.TakeNoDamage);
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        if (armorStat != null)
        {
            armorStat.RestoreArmor();
            if (armorStat.armorType.HasFlag(ArmorType.Single)|| armorStat.armorType.HasFlag(ArmorType.Multiple))
            {
                EnableState(EnemyState.Amored);
            }
        }
        gameObject.tag = "Enemy";
        if (enemyNavMeshMovement != null)
        {
            //Debug.Log("Turn back on YOU BITCH");
            NavMeshAI nmAI = GetComponent<NavMeshAI>();
            nmAI.Enable(true);
            nmAI.SetDestination();
        }
        else if (enemyPathMovement != null)
        {
            GetComponent<EnemyMovement>().enabled = true;
        }
        enemyColor.enabled = true;
        //gameObject.SetActive(true);
        //throw new NotImplementedException();
    }
    public void When_Insta_kill()//remove soon??
    {
        GameObject sth = Instantiate(Displayer, transform.position, Quaternion.identity);
        sth.transform.GetChild(0).GetComponentInChildren<TMP_TextController>().SetText("Insta Kill!!!");
        Destroy(sth, 0.5f);
        Handler.RemoveALLDebuff();
        Die();//pooling
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
    #region weaken 
    public void Weaken(StatValueType stat)
    {
        modifier = stat;
        EnableState(EnemyState.Weaken);
    }
    public StatValueType GetWeakenValue()
    {
        return modifier;
    }
    public void EndWeaken()
    {
        DisableState(EnemyState.Weaken);
    }
    #endregion
    
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
