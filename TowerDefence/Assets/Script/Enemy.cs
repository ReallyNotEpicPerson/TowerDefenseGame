using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#region ENUM
[System.Flags]
public enum EnemyState
{
    None = 0, FirstHit = 1 << 0, Slow = 1 << 1, Weaken = 1 << 2, StandStill = 1 << 3, Amored = 1 << 5, TakeNoDamage = 1 << 6, Fear = 1 << 7, HealingTime = 1 << 8,

    //ArmorBurn = Slow | Amored,
    //Slow2=1<<2,?
    //Slow3=1<<3,?
}
[System.Flags]
public enum EnemyType
{
    None = 0,//just a normal enemy
    FirstHitSpeedBoost = 1 << 0,
    Revive = 1 << 1,
    Healing = 1 << 2,
    Invisible = 1 << 3,
    //Necromancy = 1 << 4, //broke
    ImmunityToAll = 1 << 5, ImmuneToPoison = 1 << 6, ImmuneToSlow = 1 << 7, ImmuneToFire = 1 << 8, ImmuneToFear = 1 << 9,
    ImmunityToInsta_Kill = 1 << 10, ImmunityToWeaken = 1 << 11, ImmunityToArmorBreaking = 1 << 12,
    //teleport?
}
public enum MovementType
{
    FixedPath,
    NavMesh,
}
#endregion 
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public EnemyState enemyState;
    #region EnemyStat
    //public float startSpeed = 10f;
    public float startHealth = 100f;
    public Armor armorStat;

    public bool UsePathFinding;
    private NavMeshAI enemyNavMeshMovement;
    private EnemyMovement enemyPathMovement;
    public float ChanceToEvade = 0.1f;
    //[HideInInspector]
    //public float speed;
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
    [SerializeField] private SpecialFX specialFX;

    private StatValueType modifier;
    //private string ogTag;
    private Color ogHealthBarColor;
    StatUI_InGame statUI;
    #region run in the editor
    private void OnValidate()
    {
        if (healthBar == null)//change
        {
            healthBar = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        }
        if (TryGetComponent(out enemyNavMeshMovement))
        {
            UsePathFinding = true;
        }
        else if (TryGetComponent(out enemyPathMovement))
        {
            UsePathFinding = false;
        }
        if (!TryGetComponent(out armorStat))
        {
            //Debug.LogError("you forgot the armor bro");
            return;
        }
        /*if (enemyType.HasFlag(EnemyType.Necromancy) && enemyType.HasFlag(EnemyType.Healing))
        {
            Debug.LogError("NO,only one buddy");
        }*/
        if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
        {
            EnableState(EnemyState.Amored);
        }
        else if (armorStat.armorType.HasFlag(ArmorType.None))
        {
            DisableState(EnemyState.Amored);
        }
        //TryGetComponent(out fxManager);
    }
    #endregion
    private void Awake()
    {
        enemyColor = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().enabled = true;
        if (TryGetComponent(out enemyNavMeshMovement))
        {
            UsePathFinding = true;
        }
        else if (TryGetComponent(out enemyPathMovement))
        {
            UsePathFinding = false;
        }
        TryGetComponent(out fxManager);
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
    }
    void Start()
    {
        //speed = startSpeed;
        health = startHealth;
        //Ded = Instantiate(dedFX, transform.position, Quaternion.identity);
        //Ded.SetActive(false);
        //ogTag = gameObject.tag;
        ogHealthBarColor = healthBar.color;
    }
    #region Damaging
    public bool TakeDamage(float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        if (enemyState.HasFlag(EnemyState.TakeNoDamage))
        {
            return false;
        }
        if (!enemyState.HasFlag(EnemyState.FirstHit))
        {
            EnableState(EnemyState.FirstHit);
            if (enemyType.HasFlag(EnemyType.Invisible))
            {
                fxManager.Invisible(this);
            }
            if (enemyType.HasFlag(EnemyType.Healing))
            {
                Debug.Log("Activate Heal");
                GetComponent<Cast>().FindTarget();
            }
            if (enemyType.HasFlag(EnemyType.FirstHitSpeedBoost))
            {
                fxManager.Slow(this);
            }
            /*if (enemyType.HasFlag(EnemyType.Necromancy))
            {
                Debug.Log("Raise underling !");
                GetComponent<Cast>().SpawnEnemy();
            }*/
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
            case DamageDisplayerType.Poisoned:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.Poisoned);
                break;
            case DamageDisplayerType.ArmorPenetration:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.ArmorPenetration);
                break;
            case DamageDisplayerType.NO:
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
            if (enemyType.HasFlag(EnemyType.Revive) && fxManager != null)
            {
                fxManager.Revive(this);
                Debug.Log("He live this time");
                return false;
            }
            //Debug.Log("Nope .... he died");
            Die();
        }
        return true;
    }
    public bool ArmorPiercing(float amount, DamageDisplayerType type = DamageDisplayerType.Normal)
    {
        if (enemyState.HasFlag(EnemyState.TakeNoDamage))
        {
            return false;
        }
        if (!enemyState.HasFlag(EnemyState.FirstHit))
        {
            EnableState(EnemyState.FirstHit);
            if (enemyType.HasFlag(EnemyType.Invisible))
            {
                fxManager.Invisible(this);
            }
            if (enemyType.HasFlag(EnemyType.Healing))
            {
                Debug.Log("Activate Heal");
                GetComponent<Cast>().FindTarget();
            }
            if (enemyType.HasFlag(EnemyType.FirstHitSpeedBoost))
            {
                fxManager.Slow(this);
            }
            /*if (enemyType.HasFlag(EnemyType.Necromancy))
            {
                Debug.Log("Raise underling !");
                GetComponent<Cast>().SpawnEnemy();
            }*/
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
            case DamageDisplayerType.Poisoned:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.Poisoned);
                break;
            case DamageDisplayerType.ArmorPenetration:
                DamageDisplayer.Create(transform.position, amount, DamageDisplayerType.ArmorPenetration);
                break;
            default:
                Debug.LogError("But How?");
                break;
        }
        //StartCoroutine(Shaking());
        health -= amount;
        if (health > startHealth)
        {
            health = startHealth;
        }
        healthBar.fillAmount = health / startHealth;
        if (health <= 0 && !isDead)
        {
            if (enemyType.HasFlag(EnemyType.Revive) && fxManager != null)
            {
                fxManager.Revive(this);
                Debug.Log("He wont die this time");
                return false;
            }
            //Debug.Log("He ded , oopsie daisy");
            Die();
        }
        return true;
    }
    #endregion
    #region Handler
    public void AddDebuff(BaseEffect baseEffect)//use this instead,like EVERY TIME
    {
        Handler.AddDebuff(baseEffect, gameObject);
        //Debug.Log("Yes");
    }
    public bool HaveThis(string ID)
    {
        return Handler.Contain(ID);
    }
    public void RemoveALLDebuff()
    {
        Handler.RemoveALLDebuff();
    }
    #endregion
    #region additional Special Effect
    public bool ContainFX(string ID)
    {
        return specialFX.HaveThisFX(ID);
    }
    public void AddFX(TimedEffect fx)
    {
        specialFX.AddFX(fx);
    }
    public void RemoveFX(string ID)
    {
        specialFX.RemoveFX(ID);
    }
    public void RemoveAllFX()
    {
        specialFX.RemoveAllFX();
    }
    public void OneOffFX(TimedEffect fx)
    {
        specialFX.AddFX(fx);
    }
    #endregion
    #region EnemyDeaths
    public bool IsDed()//idk
    {
        return isDead;
    }
    void Die()
    {
        RemoveALLDebuff();
        isDead = true;
        PlayerStat.moneyInGame += worth;
        //Ded.SetActive(true);
        Destroy(Instantiate(dedFX, transform.position, Quaternion.identity), 0.5f);
        //WaitFor((int)(2*FloatToIntRate));
        TheSpawner.numOfEnemies--;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(FadeToBlack());
        //Ded.SetActive(false);
        //gameObject.SetActive(false);
    }
    public IEnumerator FadeToBlack()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            enemyColor.color = new Color(1, 1, 1, i);
            yield return null;
        }
        gameObject.SetActive(false);
    }
    public void FakeDeath()
    {
        Debug.Log("Sleep");
        //gameObject.tag = "Untagged";
        GetComponent<Collider2D>().enabled = false;
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
            if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
            {
                EnableState(EnemyState.Amored);
            }
        }
        //gameObject.tag = "Enemy";
        GetComponent<Collider2D>().enabled = true;
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
            enemyColor.flipX = !enemyColor.flipX;     
        }
        else
        {
            enemyPathMovement.Turn();
        }
    }
    public void IncreaseSpeed(StatModifier mod)
    {
        //EnableState(EnemyState.Fear);
        if (UsePathFinding)
        {
            //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
            enemyNavMeshMovement.AddSpeedMod(mod);
        }
        else
        {
            enemyPathMovement.AddSpeedMod(mod);
        }
    }
    public void SlowDown(StatModifier mod)
    {
        if (UsePathFinding)
        {
            //Debug.Log("Mod value: "+ mod.value+" mod type :"+mod.type);
            enemyNavMeshMovement.AddSpeedMod(mod);
        }
        else
        {
            enemyPathMovement.AddSpeedMod(mod);
        }
        //Debug.Log("Work!,GOD DAMMIT");
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
            enemyPathMovement.RemoveMod(source);
        }
    }/*
    public void StopForASec()
    {
        if (UsePathFinding)
        {
            StartCoroutine(Nav_SpeedSetTo0Coroutine());
        }
        else
        {
            StartCoroutine(Path_Mov_SpeedSetTo0Coroutine());
        }
    }

    public IEnumerator Nav_SpeedSetTo0Coroutine()
    {
        enemyNavMeshMovement.SetSpeed(0);
        yield return HelperClass.WaitFor(2000);
        enemyNavMeshMovement.SetSpeed();
    }
    public IEnumerator Path_Mov_SpeedSetTo0Coroutine()
    {
        enemyPathMovement;
        yield return HelperClass.WaitFor(2000);
    }*/
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
    public void SetHealthColor()
    {
        healthBar.color = ogHealthBarColor;
    }
    public void SetHealthColor(Color c)
    {
        healthBar.color = c;
    }
    public void ReverseBlackDad()
    {
        GetComponent<Collider2D>().enabled = true;
        enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, enemyColor.color.a * 2);
    }
    public void Invisible()
    {
        GetComponent<Collider2D>().enabled = false;
        enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, enemyColor.color.a / 2);
        //Debug.Log(enemyColor.color);
    }
    public void EndCast()
    {
        if (enemyType.HasFlag(EnemyType.Healing))
        {
            GetComponent<Cast>().NoMoreFindTarget();
        }
        /*if (enemyType.HasFlag(EnemyType.Necromancy))
        {
            GetComponent<Cast>().NoMoreNecromancy();
        }*/
        return;
    }
    public void ResumeCast()
    {
        if (enemyType.HasFlag(EnemyType.Healing))
        {
            GetComponent<Cast>().FindTarget();
        }
        /*if (enemyType.HasFlag(EnemyType.Necromancy))
        {
            GetComponent<Cast>().SpawnEnemy();
        }*/
        return;
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
    public bool CheckEnemyType(EnemyType et)
    {
        return enemyType.HasFlag(et);
    }
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
