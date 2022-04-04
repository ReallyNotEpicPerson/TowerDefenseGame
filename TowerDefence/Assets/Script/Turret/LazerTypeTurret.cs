using TMPro;
#if UNITY_EDITOR
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

    public LineRenderer lineRenderer;
    public ParticleSystem lazerFX;

    public string etag = "Enemy";
    private Enemy ene;
    private float damage;

    public EffectManager effectManager;

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
    private void Awake()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }
    private void Start()
    {
        timer = rate.baseValue;
    }
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(etag);
        float shortestDis = Mathf.Infinity;
        GameObject nearestenemy = null;
        foreach (GameObject enemy in enemies)//check distance
        {
            float DisToenenmy = Vector3.Distance(transform.position, enemy.transform.position);
            if (DisToenenmy < shortestDis)
            {
                shortestDis = DisToenenmy;
                nearestenemy = enemy;
            }
        }
        if (nearestenemy != null && shortestDis <= range.value)
        {
            target = nearestenemy.transform;
            ene = nearestenemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
        }
    }
    void Update()
    {
        if (target == null)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                lazerFX.Stop();
            }
            return;
        }
        RotateToObject();
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            LazerShoot();
            timer = rate.value;
        }
    }
    float CritDamage()
    {
        return damageOverTime.value * critDamage.baseValue;
    }
    void LazerShoot()
    {
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
                    ene.ArmorPiercing(damage, DamageDisplayerType.Critial);
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
                    ene.ArmorPiercing(damage);
                }
                else
                {
                    ene.TakeDamage(damage);
                }
                //ene.TakeDamage(damage);
            }
        }
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            lazerFX.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        lazerFX.transform.position = target.position + dir.normalized;
        if (ene.enemyType.HasFlag(EnemyType.ImmunityToAll))
        {
            return;
        }
        if (bulletType.HasFlag(BulletType.Insta_Kill))
        {
            effectManager.Insta_kill(ene);
        }
        if (bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            effectManager.Slow(ene);
        }
        if (bulletType.HasFlag(BulletType.Dots))
        {
            effectManager.Dots(ene);
        }
        if (bulletType.HasFlag(BulletType.Fear))
        {
            effectManager.Fear(ene);
        }
        if (bulletType.HasFlag(BulletType.Weaken))
        {
            effectManager.Weaken(ene);
        }
        if (bulletType.HasFlag(BulletType.DisableArmor))
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
    public void DestroyLeftoverUI()
    {
        //Destroy(hiddenUI);
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
