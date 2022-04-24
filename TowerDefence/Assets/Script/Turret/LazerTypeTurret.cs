#if UNITY_EDITOR
using System.Collections.Generic;
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
    //private Enemy ene;
    private Enemy enemies;
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
        transform.Find("Range").localScale *= range.baseValue;
        TryGetComponent(out fxManager);
        //transform.GetChild(1).localScale = Vector3.one;
        //transform.GetChild(1).localScale *= range.baseValue;
    }
    private void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }

    void UpdateTarget()
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
            timer = 1/rate.value;
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
        return damageOverTime.value * critDamage.baseValue;
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
    public void ReduceRate(StatModifier mod)
    {
        rate.AddModifier(mod);
    }
    public void UndoModification(object source)
    {
        rate.RemoveAllModifiersFromSource(source);
    }
    public StringBuilder TurretStat(LazerTypeTurret upgradeVersion)
    {
        StringBuilder text = new StringBuilder();

        if (upgradeVersion.damageOverTime.baseValue > this.damageOverTime.baseValue)
        {
            text.Append("Damage: " + damageOverTime.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.damageOverTime.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.damageOverTime.baseValue < this.damageOverTime.baseValue)
        {
            text.Append("Damage: " + damageOverTime.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.damageOverTime.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.critChance.baseValue > this.critChance.baseValue)
        {
            text.Append("Crit%: " + critChance.baseValue + "%" + "->" + $"<color=#00ff00ff>{upgradeVersion.critChance.baseValue}</color>" + "%" + "\n");
        }
        if (upgradeVersion.critChance.baseValue < this.critChance.baseValue)
        {
            text.Append("Crit%: " + critChance.baseValue + "%" + "->" + $"<color=#00ff0000>{upgradeVersion.critChance.baseValue}</color>" + "%" + "\n");
        }
        if (upgradeVersion.rate.baseValue > rate.baseValue)
        {
            text.Append("Damage rate: " + rate.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.rate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.rate.baseValue < rate.baseValue)
        {
            text.Append("Damage rate: " + rate.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.rate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue > range.baseValue)
        {
            text.Append("Range: " + range.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue < range.baseValue)
        {
            text.Append("Range: " + range.baseValue + "->" + $"<color=#00ff0000>{upgradeVersion.range.baseValue}</color>" + "\n");
        }      
        if (upgradeVersion.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up_SL = upgradeVersion.fxManager.GetSlowEffect() as SlowEffect;
            if (bulletType.HasFlag(BulletType.SlowPerSecond))
            {
                SlowEffect pre_SL = this.fxManager.GetSlowEffect() as SlowEffect;
                if (up_SL.chance > pre_SL.chance)
                {
                    text.Append("Chance: " + pre_SL.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up_SL.chance * 100}</color>" + "%" + "\n");
                }
                if (up_SL.chance < pre_SL.chance)
                {
                    text.Append("Chance: " + pre_SL.chance * 100 + "%" + "->" + $"<color=#00ff0000>{up_SL.chance * 100}</color>" + "%" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value > pre_SL._slowPercentage.statValue.value)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{up_SL._slowPercentage.statValue.value * 100}</color>" + "%" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value < pre_SL._slowPercentage.statValue.value)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff0000>{up_SL._slowPercentage.statValue.value * 100}</color>" + "%" + "\n");
                }
                if (up_SL._duration > pre_SL._duration)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#00ff00ff>{up_SL._duration}</color>" + "s" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value < pre_SL._slowPercentage.statValue.value)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#00ff0000>{up_SL._duration}</color>" + "s" + "\n");
                }
            }
            else
            {
                if (up_SL.chance < 1)
                {
                    text.Append(up_SL.chance * 100 +"%" + " to ");
                }
                text.Append(up_SL.description + " " + $"<color=#00ff0000>{up_SL._slowPercentage.statValue.value * 100}</color>" + "%" + " in " + up_SL._duration + "s" + "\n");//+ $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
                if (up_SL.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up_SL.stackTime + "\n");
                }
            }
        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.Dots))
        {

        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.Fear))
        {

        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.Weaken))
        {

        }
        if (upgradeVersion.bulletType.HasFlag(BulletType.ArmorBreaking))
        {

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
