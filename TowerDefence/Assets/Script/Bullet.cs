#if UNITY_EDITOR
#endif
using UnityEngine;

public class Bullet : BaseBulletClass
{
    private TurretType turretType;
    [SerializeField] EffectManager effectManager;
    [SerializeField] private CharacterStat explosionRadius;
    public float accuracy=1;
    float damage;

    public void Seek(Transform _target, TurretType t)
    {
        target = _target;
        turretType = t;
    }
    void Update()//make bullet move and shit
    {
        if (target == null)
        {
            Destroy(gameObject);
            //Bullet.SetActive(false);
            return;
        }
        Vector3 dir = target.position - transform.position;
        float DistantThisFrame = speed.baseValue * Time.deltaTime;
        if (dir.magnitude <= DistantThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * DistantThisFrame, Space.World);
        transform.LookAt(target);
    }
    void HitTarget()
    {
        GameObject Fx = Instantiate(ImpactFx, transform.position, target.rotation);
        Destroy(Fx, 1.5f);

        if (bulletType.HasFlag(BulletType.Explode))
        {
            Explode();
        }
        else
        {
            Damage(target);
        }
        Destroy(gameObject);
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius.baseValue);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(Taggu))
            {
                Damage(col.transform);
            }
        }
    }
    void Damage(Transform enemy)
    {
        Enemy ene = enemy.GetComponent<Enemy>();
        if (accuracy * Random.value >= ene.ChanceToEvade * Random.value)
        {
            if (!ene.enemyState.HasFlag(EnemyState.FirstHit))
            {
                ene.EnableState(EnemyState.FirstHit);
            }
            DamageDisplayer.Create(ene.transform.position, "MISS");
            return;
        }
        StatValueType Modifier = ene.GetWeakenValue();
        if (ene != null)
        {
            if (Random.value < critChance.value)
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
                    /*if (ene.TakeDamage(damage, DamageDisplayerType.Critial);
                    {
                        return;
                    }*/
                    ene.ArmorPiercing(damage, DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(damage, DamageDisplayerType.Critial);
                }
            }
            else
            {
                damage = bulletDamage.value;
                if (ene.enemyState.HasFlag(EnemyState.Weaken))
                {
                    if (Modifier.modType == StatModType.Flat)
                    {
                        damage += Modifier.statValue.value;
                    }
                    else if (Modifier.modType == StatModType.PercentAdd || Modifier.modType == StatModType.PercentAdd)
                    {
                        damage *= (1 + Modifier.statValue.value);
                    }
                }
                if (bulletType.HasFlag(BulletType.ArmorPiercing))
                {
                    ene.ArmorPiercing(damage,DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(damage);
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
        }
    }
    float CritDamage()
    {
        return bulletDamage.baseValue * critDamage.baseValue;
    }
    /*
    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), explosionRadius);
            //Gizmos.DrawWireSphere(transform.position,explosionRadius);    
        }
    #endif*/
}
