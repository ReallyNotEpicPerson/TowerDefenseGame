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

    private AudioSource audioSource;
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
    public override void Awake()
    {
        base.Awake();
        TryGetComponent(out audioSource);
    }
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
        timer = 0;
    }
    void Update()
    {
        if (target.Count == 0 || target[0] == null)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                lazerFX.Stop();
                Glow.Stop();
                audioSource.Stop();
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
            audioSource.Play();
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
        text.Append(damageOverTime.value);
        //text.Append("Damage:" + damageOverTime.value + "\n");
        return text;
    }
    public StringBuilder GetROF()
    {
        StringBuilder text = new StringBuilder();
        text.Append(rate.value);
        //text.Append("Rate:" + rate.value + "\n");
        return text;
    }
    public StringBuilder GetRange()
    {
        StringBuilder text = new StringBuilder();
        text.Append(range.value);
        //text.Append("Range:" + range.value + "\n");
        return text;
    }
    public StringBuilder GetCritChance()
    {
        StringBuilder text = new StringBuilder();
        text.Append(critChance.value * 100 + "%");
        return text;
    }
    public StringBuilder GetCritDamage()
    {
        StringBuilder text = new StringBuilder();
        text.Append("+" + critDamage.value * 100 + "%");
        return text;
    }
    public StringBuilder GetStatusEffect()
    {
        StringBuilder text = new StringBuilder();
        if (bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            text.Append(GetSlow());
        }
        if (bulletType.HasFlag(BulletType.Dots))
        {
            text.Append(GetDOTS());
        }
        if (bulletType.HasFlag(BulletType.Fear))
        {
            text.Append(GetFear());
        }
        if (bulletType.HasFlag(BulletType.Weaken))
        {
            text.Append(GetWeaken());
        }
        if (bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            text.Append(GetArmorBreaking());
        }
        if (bulletType.HasFlag(BulletType.PiercingShot))
        {
            text.Append("Ignore ememy armor" + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.CanShootWhenBuy))
        {
            text.Append("Cant be place on the field,but can shoot when bought" + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.CanSeeInvisibleUnit))
        {
            text.Append("Can see invisible enemy" + "\n");
        }
        if (text.Length == 0)
        {
            text.Append("Ability" + "\n" + "None" + "\n");
        }
        return text;
    }
    public StringBuilder GetSlow()
    {
        StringBuilder text = new StringBuilder();

        SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
        if (SE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{SE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        if (SE.ID.Contains("SL"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " for " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("STUN"))
        {
            text.Append(SE.description + " for " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("TUR"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " each shot" + "\n");
        }
        if (SE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + SE.stackTime + "\n");
        }
        return text;
    }
    public StringBuilder GetDOTS()
    {
        StringBuilder text = new StringBuilder();

        DotsEffect DE = fxManager.GetDOTSEffect() as DotsEffect;

        if (DE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{DE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        text.Append(DE.description + " " + $"<color=#00ff00ff>{DE.damagePerRate.value/DE.rate.value}</color>" + " per second" + " for " + $"<color=#00ff00ff>{ DE._duration + "s"}</color>" + "\n");
        if (DE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{DE.stackTime}</color>" + "\n");
            text.Append("Damage increase rate :" + $"<color=#00ff00ff>{DE.damageIncreaseRate.statValue.value}</color>" + "\n");
            text.Append("time reduction rate :" + "-" + $"<color=#00ff00ff>{DE.rateIncrease.statValue.value}</color>" + "s" + "\n");
        }
        return text;
    }
    public StringBuilder GetFear()
    {
        StringBuilder text = new StringBuilder();
        FearEffect FE = fxManager.GetFearEffect() as FearEffect;
        if (FE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{FE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        text.Append(FE.description + " for " + $"<color=#00ff00ff>{ FE._duration + "s"}</color>" + "\n");
        if (FE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{FE.stackTime}</color>" + "\n");
        }
        return text;
    }
    public StringBuilder GetWeaken()
    {
        StringBuilder text = new StringBuilder();

        Weaken WE = fxManager.GetWeakenEffect() as Weaken;
        if (WE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{WE.chance * 100 + "%"}</ color > " + " chance" + " to ");
        }
        text.Append(WE.description);
        if (WE.extraDamageTaken.modType.HasFlag(StatModType.Flat))
        {
            text.Append($"<color=#00ff00ff>{"+" + WE.extraDamageTaken.statValue.value}</color>" + " Damage " + "\n");
        }
        else if (WE.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || WE.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
        {
            text.Append($"<color=#00ff00ff>{"+" + WE.extraDamageTaken.statValue.value + "%"}</color>" + " Damage " + "\n");
        }
        if (WE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{WE.stackTime}</color>" + "\n");
            text.Append("Extra Damage taken rate :" + " +" + WE.increaseRate.statValue.value + "\n");
            //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
        }
        return text;
    }
    public StringBuilder GetArmorBreaking()
    {
        StringBuilder text = new StringBuilder();

        ArmorBreaking ABE = fxManager.GetWeakenEffect() as ArmorBreaking;
        if (ABE.chance < 1)
        {
            text.Append(ABE.chance * 100 + " to ");
        }
        text.Append(ABE.description + " for " + $"<color=#00ff00ff>{ ABE._duration + "s"}</color>" + "\n");

        return text;
    }

    public Sprite StatusEffectSprite()
    {
        switch (bulletType)
        {
            case BulletType.SlowPerSecond:
                SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
                return SE.Icon;
            case BulletType.Dots:
                DotsEffect DE = fxManager.GetDOTSEffect() as DotsEffect;
                return DE.Icon;
            case BulletType.Fear:
                FearEffect FE = fxManager.GetFearEffect() as FearEffect;
                return FE.Icon;
            case BulletType.Insta_Kill:
                break;
            case BulletType.Weaken:
                Weaken WE = fxManager.GetWeakenEffect() as Weaken;
                return WE.Icon;
            case BulletType.ArmorBreaking:
                ArmorBreaking ABE = fxManager.GetWeakenEffect() as ArmorBreaking;
                return ABE.Icon;
            case BulletType.PiercingShot:
                break;
            default:
                break;
        }
        Debug.LogError("OH FUCK FUCK FUCK NO Sprite");
        return null;
    }
    public StringBuilder GetTargetingType()
    {
        StringBuilder text = new StringBuilder();
        switch (targetingType)
        {
            case TargetingType.Closest:
                text.Append("Target the closest Enemy" + "\n");
                break;
            case TargetingType.First:
                text.Append("Target the first Enemy" + "\n");
                break;
            case TargetingType.LeastHealth:
                text.Append("Target the weakest Enemy" + "\n");
                break;
            case TargetingType.MostHealth:
                text.Append("Target the strongest Enemy" + "\n");
                break;
            case TargetingType.Random:
                text.Append("Target random Enemy" + "\n");
                break;
            default:
                break;
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
