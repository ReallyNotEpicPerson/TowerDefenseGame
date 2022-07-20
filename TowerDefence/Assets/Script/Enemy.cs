using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

#region ENUM
public enum AudioType
{
    None = 0,
    PlayAtAwake = 1,
    PlayAtStart = 2,
}
[System.Flags]
public enum EnemyState
{
    None = 0, FirstHit = 1 << 0, Slow = 1 << 1, Weaken = 1 << 2, StandStill = 1 << 3, Armored = 1 << 5,
    TakeNoDamage = 1 << 6, Fear = 1 << 7, HealingTime = 1 << 8, Invisible = 1 << 9, YouEarnNothing = 1 << 10
}
[System.Flags]
public enum EnemyType
{
    None = 0,
    FirstHitSpeedBoost = 1 << 0,
    Revive = 1 << 1,
    Healing = 1 << 2,
    Invisible = 1 << 3, ImmunityToAll = 1 << 5, ImmuneToPoison = 1 << 6, ImmuneToSlow = 1 << 7, ImmuneToFire = 1 << 8,
    ImmuneToFear = 1 << 9, ImmuneToInsta_Kill = 1 << 10, ImmuneToWeaken = 1 << 11, ImmuneToArmorBreaking = 1 << 12, 
    ImmuneToStun = 1 << 13,

}
#endregion 
public class Enemy : MonoBehaviour
{
    public Sprite enemySprite;
    public EnemyType enemyType;
    public EnemyState enemyState;
    public AudioType audioType;
    #region EnemyStat
    //public float startSpeed = 10f;
    public float startHealth = 100f;
    public Armor armorStat;

    public bool UsePathFinding;
    public NavMeshAI enemyNavMeshMovement;
    public EnemyPathMovement enemyPathMovement;
    public float ChanceToEvade = 0.001f;
    //[HideInInspector]
    //public float speed;
    private float health;
    public int worth = 20;
    private int reviveTime = 0;
    #endregion

    public GameObject dedFX;
    //GameObject Ded;
    [SerializeField] private Image healthBar;

    private bool isDead = false;
    private SpriteRenderer enemyColor;

    [SerializeField] private EntityEffectHandler Handler;
    [SerializeField] private EffectManager fxManager;
    [SerializeField] private SpecialFX specialFX;

    public AudioClip Death;

    private StatValueType modifier;
    private AudioSource audioSource;
    private Animator animator;
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
        if (UsePathFinding)
        {
            TryGetComponent(out enemyNavMeshMovement);

        }
        else if (!UsePathFinding)
        {
            TryGetComponent(out enemyPathMovement);
        }
        if (!TryGetComponent(out armorStat))
        {
            //Debug.LogError("you forgot the armor bro");
            return;
        }
        if (!TryGetComponent(out audioSource))
        {
            gameObject.AddComponent(typeof(AudioSource));
        }
        if (!TryGetComponent(out animator))
        {
            gameObject.AddComponent(typeof(Animator));
        }
        /*if (enemyType.HasFlag(EnemyType.Necromancy) && enemyType.HasFlag(EnemyType.Healing))
        {
            Debug.LogError("NO,only one buddy");
        }*/
        if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
        {
            EnableState(EnemyState.Armored);
        }
        else if (armorStat.armorType.HasFlag(ArmorType.None))
        {
            DisableState(EnemyState.Armored);
        }
        //TryGetComponent(out fxManager);
    }
    #endregion
    private void Awake()
    {
        reviveTime = 0;
        TryGetComponent(out enemyColor);
        TryGetComponent(out audioSource);
        TryGetComponent(out animator);
        GetComponent<Collider2D>().enabled = true;
        if (UsePathFinding)
        {
            TryGetComponent(out enemyNavMeshMovement);
            GetComponent<EnemyPathMovement>().enabled = false;
            enemyNavMeshMovement.Enable(true);
        }
        else if (!UsePathFinding)
        {
            TryGetComponent(out enemyPathMovement);
            GetComponent<NavMeshAI>().Enable(false);
            enemyPathMovement.enabled = true;
        }
        TryGetComponent(out fxManager);
        if (audioType.HasFlag(AudioType.PlayAtAwake))
        {
            audioSource.Play();
        }
        if (!TryGetComponent(out armorStat))
        {
            //Debug.LogError("you forgot the armor bro");
            return;
        }
        if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
        {
            EnableArmor();
            EnableState(EnemyState.Armored);
        }
        else if (armorStat.armorType.HasFlag(ArmorType.None))
        {
            DisableState(EnemyState.Armored);
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
                //Debug.Log("Activate Heal");
                GetComponent<Cast>().FindTarget();
            }
            if (enemyType.HasFlag(EnemyType.FirstHitSpeedBoost))
            {
                //Debug.Log("Boost");
                animator.SetTrigger("SpeedBoost");
                fxManager.Slow(this);
            }
            /*if (enemyType.HasFlag(EnemyType.Necromancy))
            {
                Debug.Log("Raise underling !");
                GetComponent<Cast>().SpawnEnemy();
            }*/
        }
        if (enemyState.HasFlag(EnemyState.Armored))
        {
            if (armorStat.DamageArmor(amount) == false)
            {
                DisableState(EnemyState.Armored);
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
                if (fxManager.Revive(this))
                    return false;
            }
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
                //Debug.Log("Activate Heal");
                GetComponent<Cast>().FindTarget();
            }
            if (enemyType.HasFlag(EnemyType.FirstHitSpeedBoost))
            {
                animator.SetTrigger("SpeedBoost");
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
                if (fxManager.Revive(this))
                    return false;
            }
            //Debug.Log("He ded , oopsie daisy");
            Die();
        }
        return true;
    }
    public float GetHealthAmount()
    {
        return health;
    }
    public void Non_Armored()
    {
        animator.SetTrigger("NoArmor");
    }
    public void EnableArmor()
    {
        animator.SetTrigger("Armored");
    }
    public void ArmorBreak()
    {
        animator.SetTrigger("Break");
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
    public void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        RemoveALLDebuff();
        isDead = true;
        if (!enemyState.HasFlag(EnemyState.YouEarnNothing))
        {
            PlayerStat.moneyInGame += worth;
        }
        if (dedFX != null)
        {
            Destroy(Instantiate(dedFX, transform.position, Quaternion.identity), 1f);
        }
        TheSpawner.numOfEnemies--;
        if (UsePathFinding)
        {
            //GetComponent<NavMeshAI>().Enable(false);
            enemyNavMeshMovement.Enable(false);
        }
        else
        {
            enemyPathMovement.enabled = false;
        }
        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetTrigger("Die");
        }
        if (Death != null)
        {
            audioSource.PlayOneShot(Death);
        }
    }
    public void FadeToBlackCR()
    {
        StartCoroutine(FadeToBlack());
    }
    public IEnumerator FadeToBlack()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, i);
            yield return null;
        }
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public void FakeDeath()
    {
        //gameObject.tag = "Untagged";
        GetComponent<Collider2D>().enabled = false;
        if (UsePathFinding)
        {
            enemyNavMeshMovement.Enable(false);
        }
        else if (!UsePathFinding)
        {
            enemyPathMovement.enabled = false;
        }
        enemyColor.enabled = false;
        EnableState(EnemyState.TakeNoDamage);
        //GetComponent<EntityEffectHandler>().enabled=true;
        //throw new NotImplementedException();
    }
    public void Revive()
    {
        //Debug.Log("wake");
        audioSource.PlayOneShot(GameAsset.I.revive);
        RemoveALLDebuff();
        DisableState(EnemyState.TakeNoDamage);
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        if (armorStat != null)
        {
            armorStat.RestoreArmor();
            if (armorStat.armorType.HasFlag(ArmorType.Single) || armorStat.armorType.HasFlag(ArmorType.Multiple))
            {
                EnableState(EnemyState.Armored);
            }
        }
        //gameObject.tag = "Enemy";
        GetComponent<Collider2D>().enabled = true;
        if (UsePathFinding)
        {
            //Debug.Log("Turn back on YOU BITCH");
            enemyNavMeshMovement.Enable(true);
            enemyNavMeshMovement.SetDestination();
        }
        else if (!UsePathFinding)
        {
            enemyPathMovement.enabled = true;
        }
        enemyColor.enabled = true;
        reviveTime++;
        //gameObject.SetActive(true);
    }
    public void When_Insta_kill()//remove soon??
    {
        Handler.RemoveALLDebuff();
        DamageDisplayer.Create(transform.position, "INSTA KILL!!", DamageDisplayerType.Insta_kill);
        Die();//pooling
    }
    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public int GetReviveTime()
    {
        return reviveTime;
    }
    #endregion
    #region Adjusting speed
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
            enemyPathMovement.Turn_0();
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
    public void UndoSpeedBoost()
    {
        animator.SetTrigger("DoneSB");
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
    }
    public float RemainingPath()
    {
        if (UsePathFinding)
        {
            return enemyNavMeshMovement.GetDistanceRemain();
        }
        else
        {
            return enemyPathMovement.GetDistanceRemain();
        }
    }
    /*public void StopForASec()
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
    #region Weaken 
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
    #region SetDestination
    public void SetDestination(Transform spawnPoint, Transform endPoint)
    {
        enemyNavMeshMovement.SetDestination(spawnPoint, endPoint);
    }
    public void SetDestination(int path)
    {
        enemyPathMovement.SetPathIndex(path);
    }
    #endregion
    #region Color
    public void SetHealthColor()
    {
        healthBar.color = ogHealthBarColor;
    }
    public void SetHealthColor(Color c)
    {
        healthBar.color = c;
    }
    public void SetEnemyColor(Color color)
    {
        enemyColor.color = color;
    }
    #endregion
    #region Invisibility
    public void DeInvisible()
    {
        DisableState(EnemyState.Invisible);
        enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, 1);
    }
    public bool IsInvisible()
    {
        return Handler.StillHaveInvisibility();
    }

    public void Invisible()
    {
        EnableState(EnemyState.Invisible);
        enemyColor.color = new Color(enemyColor.color.r, enemyColor.color.g, enemyColor.color.b, 0.5f);
        //Debug.Log(enemyColor.color);
    }
    #endregion
    #region Cast
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
    #endregion 
    #region function to get stats
    public Sprite GetSprite()
    {
        return enemySprite;
    }
    public StringBuilder GetHealth()
    {
        StringBuilder text = new StringBuilder();
        text.Append(startHealth);
        return text;
    }
    public StringBuilder GetSpeed()
    {
        StringBuilder text = new StringBuilder();
        text.Append(enemyPathMovement.startSpeed.value);
        return text;
    }
    public StringBuilder GetChaceToInvade()
    {
        StringBuilder text = new StringBuilder();
        text.Append(ChanceToEvade * 100 + "%");
        return text;
    }
    public StringBuilder GetWorth()
    {
        StringBuilder text = new StringBuilder();
        text.Append(worth);
        return text;
    }
    public StringBuilder GetArmorHealth()
    {
        StringBuilder text = new StringBuilder();
        text.Append(armorStat.GetArmorHeath());
        return text;
    }
    public StringBuilder GetArmorDamageReduction()
    {
        StringBuilder text = new StringBuilder();
        text.Append(armorStat.GetArmorDamageRedution() + "%");
        return text;
    }
    public StringBuilder GetArmorLayer()
    {
        StringBuilder text = new StringBuilder();
        text.Append(armorStat.GetArmorLayer());
        return text;
    }
    public StringBuilder Ability()
    {
        StringBuilder text = new StringBuilder();
        text.Append($"<color=#ffffffff>{"Special Ability:"}</color>" + "\n");
        if (enemyType.HasFlag(EnemyType.FirstHitSpeedBoost))
        {
            text.Append($"<color=#00ff00ff>{"Speed boost: "}</color>" + "Increase speed after first hit" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.Revive))
        {
            Revive RE = fxManager.GetReviveEffect() as Revive;
            text.Append($"<color=#00ff00ff>{"Revive: "}</color>" + "Have a " + RE.chance * 100 + "% chance to revive after death" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.Invisible))
        {
            text.Append($"<color=#00ff00ff>{"Invisibility: "}</color>" +
                "Become completely invisible for " + fxManager.GetInvisibleEffect().duration + "s");
        }
        if (enemyType.HasFlag(EnemyType.ImmunityToAll))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to all status effect" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToSlow))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Slow" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToStun))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Stun" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToPoison))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Poison" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToFire))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Fire" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToArmorBreaking))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Armor breaking" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToWeaken))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Weaken" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToFear))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to Fear" + "\n");
        }
        if (enemyType.HasFlag(EnemyType.ImmuneToInsta_Kill))
        {
            text.Append($"<color=#00ff00ff>{"Immunity: "}</color>" + "Immune to InstaKill" + "\n");
        }
        if (text.Length == 0)
        {
            text.Append("None" + "\n");
        }
        return text;
    }
    #endregion
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