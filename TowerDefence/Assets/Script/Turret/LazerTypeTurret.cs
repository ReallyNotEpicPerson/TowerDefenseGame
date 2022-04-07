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
    public float accuracy=1f;
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
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        float shortestDis = Mathf.Infinity;
        Collider2D nearestCol = null;
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent<Enemy>(out _))
            {
                float DisToenenmy = Vector3.Distance(transform.position, col.transform.position);//use Distancesquared??
                if (DisToenenmy < shortestDis)
                {
                    shortestDis = DisToenenmy;
                    nearestCol = col;
                }
            }
        }
        if (nearestCol != null && shortestDis <= range.value)
        {
            target = nearestCol.transform;
            //Debug.Log("Final " + nearestCol.name);
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
        if (accuracy * Random.value <= ene.ChanceToEvade * Random.value)
        {
            DamageDisplayer.Create(ene.transform.position, "MISS");
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
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            lazerFX.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        lazerFX.transform.position = target.position + dir.normalized;
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
