#if UNITY_EDITOR
#endif
using TMPro;
using UnityEngine;

public class Bullet : BaseBulletClass
{
    private TurretType turretType;
    [SerializeField] EffectManager effectManager;
    [SerializeField] private CharacterStat explosionRadius;

    public void Seek(Transform _target, TurretType t)
    {
        target = _target;
        turretType = t;
    }
    void Update()
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
        StatValueType Modifier = ene.GetWeakenValue();
        float damage;
        if (ene != null)
        {
            if (Random.value < critChance.baseValue)
            {
                damage = CritDamage() ;
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
                ene.TakeDamage(damage, DamageDisplayerType.Critial);

            }
            else
            {
                damage = bulletDamage.baseValue;
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
                ene.TakeDamage(damage);
            }
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
            if (bulletType.HasFlag(BulletType.Burn))
            {
                effectManager.Burn(ene);
            }
            if (bulletType.HasFlag(BulletType.Fear))
            {
                effectManager.Fear(ene);
            }
            if (bulletType.HasFlag(BulletType.Weaken))
            {
                effectManager.Weaken(ene);
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
