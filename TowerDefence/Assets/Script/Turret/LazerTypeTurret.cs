#if UNITY_EDITOR
using System.Text;
using UnityEditor;
#endif
using UnityEngine;

public class LazerTypeTurret : BaseTurretStat
{
    public BulletType bulletType;
    [Header("LazerTypeStat")]
    public CharacterStat critChance;
    public CharacterStat critDamage;
    public CharacterStat damageOverTime;
    public CharacterStat rate;
    private float timer;
    public Transform firePoint;
    [Header("Effects")]
    public ParticleSystem lazerFX;
    public ParticleSystem Glow;
    public EffectManager fxManager;
    public LineRenderer lineRenderer;

    //public GameObject lazerBeam;//Upgrade
    //private List<LineRenderer> lineRenderers = new List<LineRenderer>(); //upgrade 
    private float damage;
    /*public override void Awake()
    {
        base.Awake();
        for (int i = 0; i < numberOfTarget; i++)
        {
            lineRenderers.Enqueue(Instantiate(lazerBeam, gameObject.transform).GetComponent<LineRenderer>());
        }
    }*/

    public void OnValidate()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (lazerFX == null)
        {
            lazerFX = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
        }
        transform.Find("Range").localScale = Vector3.one;
        transform.Find("Range").localScale *= range.value;
        TryGetComponent(out fxManager);
    }
    public override void Start()
    {
        base.Start();
        timer = 1 / rate.value;
    }
    void Update()
    {
        if (target.Count == 0)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                lazerFX.Stop();
                Glow.Stop();
            }
            return;
        }
        RotateToObject();
        LazerBeam();
        if (timer <= 0)
        {
            Shoot();
            timer = 1 / rate.value;
        }
        timer -= Time.deltaTime;
    }
    public void Shoot()
    {
        for (int j = 0; j < target.Count; j++)
        {
            LazerShoot(target[j].GetComponent<Enemy>());
        }

    }/*
    public void LazerToward(int tar)
    {
        LineRenderer lr = lineRenderers.Dequeue();
        if (!lineRenderer.enabled)
        {
            lr.enabled = true;
            lazerFX.Play();
        }
        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, target[tar].position);
        lineRenderers.Enqueue(lr);
        //Vector3 dir = firePoint.position - target[tar].position;
        //lazerFX.transform.position = target[tar].position + dir.normalized;
    }*/
    public void LazerBeam()
    {
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            lazerFX.Play();
            Glow.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target[0].position);

        //Vector3 dir = firePoint.position - target[0].position;
        lazerFX.transform.position = target[0].position;
    }
    float CritDamage()
    {
        return damageOverTime.value * critDamage.value;
    }
    void LazerShoot(Enemy ene)
    {
        if (Random.value <= ene.ChanceToEvade)
        {
            DamageDisplayer.Create(ene.transform.position);
            return;
        }
        StatValueType Modifier = ene.GetWeakenValue();
        if (ene != null)
        {
            if (Random.value <= critChance.value)
            {
                damage = CritDamage();
                if (ene.enemyState.HasFlag(EnemyState.Weaken))
                {
                    if (Modifier.modType == StatModType.Flat)
                    {
                        damage += Modifier.statValue.value;
                    }
                    else if (Modifier.modType == StatModType.PercentAdd || Modifier.modType == StatModType.PercentMult)
                    {
                        damage *= (1 + Modifier.statValue.value);
                    }
                }
                if (bulletType.HasFlag(BulletType.PiercingShot))
                {
                    ene.ArmorPiercing(damage, DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(damage, DamageDisplayerType.Critial);
                }
                //ene.TakeDamage(damage, DamageDisplayerType.Critial);
            }
            else
            {
                damage = damageOverTime.value;
                if (ene.enemyState.HasFlag(EnemyState.Weaken))
                {
                    if (Modifier.modType == StatModType.Flat)
                    {
                        damage += Modifier.statValue.value;
                    }
                    else if (Modifier.modType == StatModType.PercentAdd || Modifier.modType == StatModType.PercentMult)
                    {
                        damage *= (1 + Modifier.statValue.value);
                    }
                }
                if (bulletType.HasFlag(BulletType.PiercingShot))
                {
                    ene.ArmorPiercing(damage, DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(damage);
                }
                //ene.TakeDamage(damage);
            }
        }
        //status effect 
        if (ene.enemyType.HasFlag(EnemyType.ImmunityToAll))
        {
            return;
        }
        if (!ene.CheckEnemyType(EnemyType.ImmunityToInsta_Kill) && bulletType.HasFlag(BulletType.Insta_Kill))
        {
            fxManager.Insta_kill(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmuneToSlow) && bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            fxManager.Slow(ene);
        }
        if (bulletType.HasFlag(BulletType.Dots))
        {
            fxManager.Dots(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmuneToFear) && bulletType.HasFlag(BulletType.Fear))
        {
            fxManager.Fear(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmunityToWeaken) && bulletType.HasFlag(BulletType.Weaken))
        {
            fxManager.Weaken(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmunityToArmorBreaking) && bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            fxManager.DisableArmor(ene);
        }
        //lazerFX.transform.rotation = Quaternion.LookRotation(dir);
    }
    /*void RotateToObj2()
    {
        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        Debug.DrawRay(transform.position, target.transform.position);
    }*/
    #region function for making changes to turret stat
    //check if another support turret is here
    public bool CheckDamageModsource(object source)
    {
        return damageOverTime.HaveSameType(source);
    }
    public bool CheckRateModsource(object source)
    {
        return rate.HaveSameType(source);
    }
    public bool CheckRangeModsource(object source)
    {
        return range.HaveSameType(source);
    }
    public bool CheckCritDamageModsource(object source)
    {
        return critDamage.HaveSameType(source);
    }
    public bool CheckCritChanceModsource(object source)
    {
        return critChance.HaveSameType(source);
    }
    // add mod
    public void AddDamageMod(StatModifier mod)
    {
        damageOverTime.AddingOneInstance(mod);
        Debug.Log(damageOverTime.value);
    }
    public void AddingOneInstanceRateMod(StatModifier mod)
    {
        rate.AddingOneInstance(mod);
    }
    public void AddRateMod(StatModifier mod)
    {
        rate.AddingOneInstance(mod);
        Debug.Log(damageOverTime.value);
    }
    public void AddRangeMod(StatModifier mod)
    {
        range.AddingOneInstance(mod);
    }
    public void AddCritDamageMod(StatModifier mod)
    {
        critDamage.AddingOneInstance(mod);
    }
    public void AddCritChanceMod(StatModifier mod)
    {
        critChance.AddingOneInstance(mod);
    }
    //remove mod
    public void UndoDamageModification(object source)
    {
        damageOverTime.RemoveAllModifiersFromSource(source);
    }
    public void UndoRateModification(object source)
    {
        rate.RemoveAllModifiersFromSource(source);
    }
    public void UndoRangeModification(object source)
    {
        range.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritDamageModification(object source)
    {
        critDamage.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritChanceModification(object source)
    {
        critChance.RemoveAllModifiersFromSource(source);
    }

    #endregion
    public StringBuilder GetDamage()
    {
        StringBuilder text = new StringBuilder();
        text.Append("Damage:" + damageOverTime.value + "\n");
        return text;
    }
    public StringBuilder GetROF()
    {
        StringBuilder text = new StringBuilder();
        text.Append("Rate:" + rate.value + "\n");
        return text;
    }
    public StringBuilder GetRange()
    {
        StringBuilder text = new StringBuilder();
        text.Append("Range:" + range.value + "\n");
        return text;
    }
    public StringBuilder GetStatusEffect()
    {
        StringBuilder text = new StringBuilder();
        if (bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
            /*
            if (SE._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
            }
            if (SE._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
            }

            if (SE._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{SE._duration + "s"}</color>" + "\n");
            }*/
            if (SE.chance < 1)
            {
                text.Append(SE.chance * 100 + "%" + " to ");
            }
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
            if (SE.effectType.HasFlag(EffectType.StackingEffect))
            {
                text.Append("Can stack to " + SE.stackTime + "\n");
            }
        }
        /*if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
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
    public StringBuilder TurretStat(LazerTypeTurret upgradeVersion)
    {
        StringBuilder text = new StringBuilder();

        if (upgradeVersion.damageOverTime.baseValue > this.damageOverTime.baseValue)
        {
            text.Append("Damage:" + damageOverTime.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.damageOverTime.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.damageOverTime.baseValue < this.damageOverTime.baseValue)
        {
            text.Append("Damage:" + damageOverTime.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.damageOverTime.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.critChance.baseValue > this.critChance.baseValue)
        {
            text.Append("Crit%:" + critChance.baseValue + "%" + "->" + $"<color=#00ff00ff>{upgradeVersion.critChance.baseValue}</color>" + "%" + "\n");
        }
        if (upgradeVersion.critChance.baseValue < this.critChance.baseValue)
        {
            text.Append("Crit%:" + critChance.baseValue + "%" + "->" + $"<color=#00ff0000>{upgradeVersion.critChance.baseValue}</color>" + "%" + "\n");
        }
        if (upgradeVersion.rate.baseValue > rate.baseValue)
        {
            text.Append("Damage rate:" + rate.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.rate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.rate.baseValue < rate.baseValue)
        {
            text.Append("Damage rate:" + rate.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.rate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue > range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue < range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up = upgradeVersion.fxManager.GetSlowEffect() as SlowEffect;
            if (bulletType.HasFlag(BulletType.SlowPerSecond))
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
        if (upgradeVersion.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up = upgradeVersion.fxManager.GetDOTSEffect() as DotsEffect;
            if (bulletType.HasFlag(BulletType.Dots))
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
        if (upgradeVersion.bulletType.HasFlag(BulletType.Fear))
        {
            FearEffect up = upgradeVersion.fxManager.GetFearEffect() as FearEffect;
            if (bulletType.HasFlag(BulletType.Fear))
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

            }
        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.Weaken))
        {
            Weaken up = upgradeVersion.fxManager.GetWeakenEffect() as Weaken;
            if (bulletType.HasFlag(BulletType.Fear))
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
            }
        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking up = upgradeVersion.fxManager.GetWeakenEffect() as ArmorBreaking;
            if (bulletType.HasFlag(BulletType.ArmorBreaking))
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
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), range.value);
        /* Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(transform.position, Range);*/
    }
#endif
}
