#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public CharacterStat fireRate;
    [Header("List of Fire Point")]
    public List<Transform> firePoint;//keep at all cost

    private float FireCountDown = 0f;
    private EntityEffectHandler fxHandler;
    private EffectManager fxManager;
    private StatModifier modifier;

    [Header("Gameobject stuff")]
    public GameObject[] objectToRotate;
    [SerializeField] private Animator animator;
    public GameObject BulletPrefab;
    [SerializeField] private Bullet bu;
    //public string PewPewAnimation;
    //private ObjectPooler pool;
    public void OnValidate()
    {
        BulletPrefab.TryGetComponent(out bu);
        TryGetComponent(out fxManager);
        TryGetComponent(out fxHandler);
        if (firePoint.Count < 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("FirePoint"))
                {
                    firePoint.Add(transform.GetChild(i));
                }
                else
                    continue;
            }
        }
        transform.Find("Range").localScale = Vector3.one;
        transform.Find("Range").localScale *= range.value;
    }
    public override void Awake()
    {
        base.Awake();
        TryGetComponent(out fxHandler);
        TryGetComponent(out fxManager);
        //BulletPrefab.TryGetComponent(out bu);
    }

    public override void Start()
    {
        base.Start();
        FireCountDown = 1 / fireRate.value;
    }
    /*void UpdateTarget()
    {
        target.Clear();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        float shortestDis = Mathf.Infinity;
        Collider2D nearestCol = null;
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent<Enemy>(out _))
            {
                if (shootType.HasFlag(ShootType.SingleTarget))
                {
                    float DisToenenmy = Vector3.Distance(transform.position, col.transform.position);//use Distancesquared??
                    if (DisToenenmy < shortestDis)
                    {
                        shortestDis = DisToenenmy;
                        nearestCol = col;
                    }
                }
                else if (shootType.HasFlag(ShootType.MultipleTarget))
                {
                    target.Add(col.transform);
                    if (target.Count == numberOfTarget)
                    {
                        return;
                    }
                }
            }
        }
        if (shootType.HasFlag(ShootType.SingleTarget) && nearestCol != null)
        {
            target.Add(nearestCol.transform);
        }
    }*/
    void Update()
    {
        if (target.Count == 0)
        {
            return;
        }
        RotateToObject();
        if (FireCountDown <= 0f)
        {
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseSpeed))
            {
                fxHandler.AddDebuff(fxManager.GetSpeedBoostData(), gameObject);
            }
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
            {
                fxHandler.AddDebuff(fxManager.GetDamageBoostData(), gameObject);
            }
            FireCountDown = 1 / fireRate.value;
            //animator.Play("Turret Layer."+PewPewAnimation);
            Shoot();
        }
        FireCountDown -= Time.deltaTime;
    }
    public void Shoot()
    {
        animator.SetTrigger("Shoot");
        for (int i = 0; i < firePoint.Count; i++)
        {
            for (int j = 0; j < target.Count; j++)
            {
                Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
                if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
                {
                    bullet.AddDamageMod(modifier);
                }
                if (bullet != null)
                {
                    bullet.Seek(target[j]);
                }
            }
        }
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }
    public override void RotateToObject()
    {
        for (int i = 0; i < objectToRotate.Length; i++)
        {
            float angle = Mathf.Atan2(target[0].transform.position.y - objectToRotate[i].transform.position.y, target[0].transform.position.x - objectToRotate[i].transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            objectToRotate[i].transform.rotation = Quaternion.RotateTowards(objectToRotate[i].transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        }
    }
    #region function for making changes to turret stat
    //check if another support turret is here
    public bool CheckDamageModsource(object source)
    {
        return bu.bulletDamage.HaveSameType(source);
    }
    public bool CheckRateModsource(object source)
    {
        return fireRate.HaveSameType(source);
    }
    public bool CheckRangeModsource(object source)
    {
        return range.HaveSameType(source);
    }
    public bool CheckCritDamageModsource(object source)
    {
        return bu.critDamage.HaveSameType(source);
    }
    public bool CheckCritChanceModsource(object source)
    {
        return bu.critChance.HaveSameType(source);
    }
    //Add mod
    public void AddDamageMod(StatModifier mod)
    {
        bu.bulletDamage.AddModifier(mod);
        Debug.Log("Damage" + bu.bulletDamage.value);

    }
    public void AddRateMod(StatModifier mod)
    {
        fireRate.AddModifier(mod);
        Debug.Log("Fire rate" + fireRate.value);
    }
    public void AddRangeMod(StatModifier mod)
    {
        range.AddModifier(mod);
        Debug.Log("Range" + range.value);
    }
    public void AddCritDamageMod(StatModifier mod)
    {
        bu.critDamage.AddModifier(mod);
        Debug.Log("Crit damage" + fireRate.value);
    }
    public void AddCritChanceMod(StatModifier mod)
    {
        bu.critChance.AddModifier(mod);
        Debug.Log("Crit chance" + fireRate.value);
    }
    //undo
    public void UndoDamageModification(object source)
    {
        bu.bulletDamage.RemoveAllModifiersFromSource(source);
    }
    public void UndoRateModification(object source)
    {
        fireRate.RemoveAllModifiersFromSource(source);
        Debug.Log("Remove all " + fireRate.value);
    }
    public void UndoRangeModification(object source)
    {
        range.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritDamageModification(object source)
    {
        bu.critDamage.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritChanceModification(object source)
    {
        bu.critChance.RemoveAllModifiersFromSource(source);
    }
    //gay method
    public void AddingOneInstanceRateMod(StatModifier mod)
    {
        Debug.Log("Fire rate" + fireRate.value);
        fireRate.AddingOneInstance(mod);

    }
    #endregion

    public Bullet BulletStat()
    {
        return bu;
    }
    public StringBuilder GetCurrentStat()
    {
        StringBuilder text = new StringBuilder();

        text.Append("Damage:" + bu.bulletDamage.value + "\n");
        text.Append("Fire rate:" + fireRate.value + "\n");
        text.Append("Range:" + range.baseValue + "\n");
        /*if (upgradeVersion.bu.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up = upgradeVersion.fxManager.GetSlowEffect() as SlowEffect;
            if (bu.bulletType.HasFlag(BulletType.SlowPerSecond))
            {
                SlowEffect pre = this.fxManager.GetSlowEffect() as SlowEffect;
                if (up.chance > pre.chance)//better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
                }
                if (up._duration > pre._duration)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s" }</color>" + "\n");
                }
                if (up._duration < pre._duration)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + "%" + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ up._duration + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up = upgradeVersion.fxManager.GetDOTSEffect() as DotsEffect;
            if (bu.bulletType.HasFlag(BulletType.Dots))
            {
                DotsEffect pre = this.fxManager.GetDOTSEffect() as DotsEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.damagePerRate.value > pre.damagePerRate.value)//Better
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#00ff00ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.damagePerRate.value < pre.damagePerRate.value)
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#ff0000ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)//Better
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#00ff00ff>{up.rate.value + "s" }</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#ff0000ff>{up.rate.value + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up.damagePerRate.value }</color>" + " damage per " + $"<color=#00ff00ff>{ up.rate.value + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Fear))
        {
            FearEffect up = upgradeVersion.fxManager.GetFearEffect() as FearEffect;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                FearEffect pre = fxManager.GetFearEffect() as FearEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    //text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Weaken))
        {
            Weaken up = upgradeVersion.fxManager.GetWeakenEffect() as Weaken;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                Weaken pre = fxManager.GetWeakenEffect() as Weaken;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description);
                //+ $"<color=#00ff00ff>{up.extraDamageTaken.statValue}</color>");
                if (up.extraDamageTaken.modType.HasFlag(StatModType.Flat))
                {
                    text.Append($"<color=#00ff00ff>{"+" + up.extraDamageTaken.statValue.value}</color>" + "\n");
                }
                else if (up.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || up.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
                {
                    text.Append($"<color=#00ff00ff>{up.extraDamageTaken.statValue.value + "%"}</color>" + "\n");
                }
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Extra Damage taken rate :" + " +" + up.increaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking up = upgradeVersion.fxManager.GetWeakenEffect() as ArmorBreaking;
            if (bu.bulletType.HasFlag(BulletType.ArmorBreaking))
            {
                ArmorBreaking pre = fxManager.GetArmorBreakEffect() as ArmorBreaking;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
            }
        }*/
        return text;
    }
    public StringBuilder CompareTurretStat(BulletTypeTurret upgradeVersion)
    {
        StringBuilder text = new StringBuilder();
        if (upgradeVersion.bu.bulletDamage.baseValue > this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage:" + bu.bulletDamage.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletDamage.baseValue < this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage:" + bu.bulletDamage.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue > this.bu.critChance.baseValue)
        {
            text.Append("Crit%:" + bu.critChance.baseValue + "%" + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue < this.bu.critChance.baseValue)
        {
            text.Append("Crit%:" + bu.critChance.baseValue + "%" + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue > fireRate.baseValue)
        {
            text.Append("Fire rate:" + fireRate.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue < fireRate.baseValue)
        {
            text.Append("Fire rate:" + fireRate.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue > range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue < range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        //StatusEffect 
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Insta_Kill))
        {
            Insta_Kill up = upgradeVersion.fxManager.GetInsta_KillEffect() as Insta_Kill;
            if (bu.bulletType.HasFlag(BulletType.Insta_Kill))
            {
                Insta_Kill pre = this.fxManager.GetInsta_KillEffect() as Insta_Kill;
                if (up.chance > pre.chance)
                {
                    text.Append("Insta-kill Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%"}</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Insta-kill Chance:" + pre.chance * 100 + " %" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                text.Append(up.description + " " + $"<color=#2596beff>{up.chance * 100 + " %"}</color>" + "\n");
            }
            //+ $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up = upgradeVersion.fxManager.GetSlowEffect() as SlowEffect;
            if (bu.bulletType.HasFlag(BulletType.SlowPerSecond))
            {
                SlowEffect pre = this.fxManager.GetSlowEffect() as SlowEffect;
                if (up.chance > pre.chance)//better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
                }
                if (up._duration > pre._duration)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s" }</color>" + "\n");
                }
                if (up._duration < pre._duration)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + "%" + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ up._duration + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up = upgradeVersion.fxManager.GetDOTSEffect() as DotsEffect;
            if (bu.bulletType.HasFlag(BulletType.Dots))
            {
                DotsEffect pre = this.fxManager.GetDOTSEffect() as DotsEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.damagePerRate.value > pre.damagePerRate.value)//Better
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#00ff00ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.damagePerRate.value < pre.damagePerRate.value)
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#ff0000ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)//Better
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#00ff00ff>{up.rate.value + "s" }</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#ff0000ff>{up.rate.value + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up.damagePerRate.value }</color>" + " damage per " + $"<color=#00ff00ff>{ up.rate.value + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Fear))
        {
            FearEffect up = upgradeVersion.fxManager.GetFearEffect() as FearEffect;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                FearEffect pre = fxManager.GetFearEffect() as FearEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    //text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Weaken))
        {
            Weaken up = upgradeVersion.fxManager.GetWeakenEffect() as Weaken;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                Weaken pre = fxManager.GetWeakenEffect() as Weaken;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description);
                //+ $"<color=#00ff00ff>{up.extraDamageTaken.statValue}</color>");
                if (up.extraDamageTaken.modType.HasFlag(StatModType.Flat))
                {
                    text.Append($"<color=#00ff00ff>{"+" + up.extraDamageTaken.statValue.value}</color>" + "\n");
                }
                else if (up.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || up.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
                {
                    text.Append($"<color=#00ff00ff>{up.extraDamageTaken.statValue.value + "%"}</color>" + "\n");
                }
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Extra Damage taken rate :" + " +" + up.increaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking up = upgradeVersion.fxManager.GetWeakenEffect() as ArmorBreaking;
            if (bu.bulletType.HasFlag(BulletType.ArmorBreaking))
            {
                ArmorBreaking pre = fxManager.GetArmorBreakEffect() as ArmorBreaking;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
            }
        }
        return text;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), range.Value);
        /* Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(transform.position, Range);*/
    }
#endif
}