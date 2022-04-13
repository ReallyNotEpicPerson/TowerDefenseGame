#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

public class LazerTypeTurret : BaseTurretStat
{
    public BulletType bulletType;
    public CharacterStat critChance;
    public CharacterStat critDamage;
    public CharacterStat damageOverTime;
    public CharacterStat rate;
    private float timer;
    public Transform firePoint;

    public ParticleSystem lazerFX;
    public EffectManager effectManager;

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
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target[0].position);

        Vector3 dir = firePoint.position - target[0].position;
        lazerFX.transform.position = target[0].position + dir.normalized;
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
                if (bulletType.HasFlag(BulletType.ArmorPiercing))
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
                if (bulletType.HasFlag(BulletType.ArmorPiercing))
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
            effectManager.Insta_kill(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmuneToSlow) && bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            effectManager.Slow(ene);
        }
        if (bulletType.HasFlag(BulletType.Dots))
        {
            effectManager.Dots(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmuneToFear) && bulletType.HasFlag(BulletType.Fear))
        {
            effectManager.Fear(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmunityToWeaken) && bulletType.HasFlag(BulletType.Weaken))
        {
            effectManager.Weaken(ene);
        }
        if (!ene.CheckEnemyType(EnemyType.ImmunityToArmorBreaking) && bulletType.HasFlag(BulletType.DisableArmor))
        {
            effectManager.DisableArmor(ene);
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
