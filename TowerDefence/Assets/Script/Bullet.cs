#if UNITY_EDITOR
#endif
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : BaseBulletClass
{
    public GameObject theFxManager;
    public EffectManager fxManager;
    [SerializeField] private CharacterStat explosionRadius;
    float finalDamage;
    public bool quadrupleDamage = false;
    public void OnValidate()//taggu
    {
        
        fxManager = theFxManager.GetComponentInChildren<EffectManager>();
        /*if (theFxManager == null || fxManager == null)
        {
            Debug.Log(name);
        }*/
        if (!TryGetComponent(out audioSource))
        {
            gameObject.AddComponent(typeof(AudioSource));
            TryGetComponent(out audioSource);
        }
    }
    public void Seek(Transform _target)
    {
        target = _target;
    }
    public void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (bulletType.HasFlag(StatusEffectType.JustStoodStill))
        {
            return;
        }
        Vector3 dir = target.position - transform.position;
        float DistantThisFrame = bulletSpeed.baseValue * Time.deltaTime;
        if (dir.magnitude <= DistantThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * DistantThisFrame, Space.World);
    }
    void HitTarget()
    {
        GameObject Fx = Instantiate(ImpactFx, transform.position, target.rotation);
        Destroy(Fx, 1f);
        if (bulletType.HasFlag(StatusEffectType.Explode))
        {
            Explode();
        }
        else
        {
            if (bulletType.HasFlag(StatusEffectType.Cast))
            {
                EnemyCast(target.GetComponent<Enemy>());
            }
            else
            {
                Damage(target.GetComponent<Enemy>());
            }
        }
        Destroy(gameObject);
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius.value);
        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent(out Enemy enemy))
            {
                if (bulletType.HasFlag(StatusEffectType.Cast))
                {
                    EnemyCast(enemy);
                }
                else
                {
                    Damage(enemy);
                }
            }
        }
    }
    public void EnemyCast(Enemy ene)
    {
        //Enemy ene = enemy.GetComponent<Enemy>();
        if (bulletType.HasFlag(StatusEffectType.SlowPerSecond))
        {
            fxManager.Slow(ene);
        }
        if (bulletType.HasFlag(StatusEffectType.Dots))
        {
            fxManager.Dots(ene);
        }
    }
    void Damage(Enemy ene)
    {
        if (ene == null)
        {
            return;
        }
        if (Random.value <= ene.ChanceToEvade)
        {
            DamageDisplayer.Create(ene.transform.position);
            return;
        }
        StatValueType Modifier = ene.GetWeakenValue();
        if (ene != null)
        {
            if (Random.value < critChance.value)
            {
                finalDamage = CritDamage();
                if (ene.enemyState.HasFlag(EnemyState.Weaken))
                {
                    if (Modifier.modType == StatModType.Flat)
                    {
                        finalDamage += Modifier.statValue.value;
                    }
                    else if (Modifier.modType == StatModType.PercentAdd || Modifier.modType == StatModType.PercentMult)
                    {
                        finalDamage *= (1 + Modifier.statValue.value);
                    }
                }
                if (bulletType.HasFlag(StatusEffectType.PiercingShot))
                {
                    if (quadrupleDamage)
                    {
                        ene.ArmorPiercing(finalDamage * 4, DamageDisplayerType.ArmorPenetration);
                        quadrupleDamage = false;
                    }
                    else
                    {
                        ene.ArmorPiercing(finalDamage, DamageDisplayerType.ArmorPenetration);
                    }
                }
                else
                {
                    if (quadrupleDamage)
                    {
                        ene.TakeDamage(finalDamage * 4, DamageDisplayerType.Critial);
                        quadrupleDamage = false;
                    }
                    else
                    {
                        ene.TakeDamage(finalDamage, DamageDisplayerType.Critial);
                    }

                }
            }
            else
            {
                finalDamage = bulletDamage.value;
                if (ene.enemyState.HasFlag(EnemyState.Weaken))
                {
                    if (Modifier.modType == StatModType.Flat)
                    {
                        finalDamage += Modifier.statValue.value;
                    }
                    else if (Modifier.modType == StatModType.PercentAdd || Modifier.modType == StatModType.PercentAdd)
                    {
                        finalDamage *= (1 + Modifier.statValue.value);
                    }
                }
                if (bulletType.HasFlag(StatusEffectType.PiercingShot))
                {
                    if (quadrupleDamage)
                    {
                        ene.ArmorPiercing(finalDamage * 4, DamageDisplayerType.ArmorPenetration);
                        quadrupleDamage = false;
                    }
                    else
                    {
                        ene.ArmorPiercing(finalDamage, DamageDisplayerType.ArmorPenetration);
                        quadrupleDamage = false;
                    }
                }
                else
                {
                    if (quadrupleDamage)
                    {
                        ene.TakeDamage(finalDamage * 4);
                        quadrupleDamage = false;
                    }
                    else
                    {
                        ene.TakeDamage(finalDamage);
                        quadrupleDamage = false;
                    }
                }
            }
            //status effect 
            if (ene.enemyType.HasFlag(EnemyType.ImmunityToAll))
            {
                return;
            }
            if (!ene.CheckEnemyType(EnemyType.ImmuneToInsta_Kill) && bulletType.HasFlag(StatusEffectType.Insta_Kill))
            {
                fxManager.Insta_kill(ene);
            }
            if (!ene.CheckEnemyType(EnemyType.ImmuneToSlow) && bulletType.HasFlag(StatusEffectType.SlowPerSecond))
            {
                fxManager.Slow(ene);
            }
            if (bulletType.HasFlag(StatusEffectType.Dots))
            {
                fxManager.Dots(ene);
            }
            if (!ene.CheckEnemyType(EnemyType.ImmuneToFear) && bulletType.HasFlag(StatusEffectType.Fear))
            {
                fxManager.Fear(ene);
            }
            if (!ene.CheckEnemyType(EnemyType.ImmuneToWeaken) && bulletType.HasFlag(StatusEffectType.Weaken))
            {
                fxManager.Weaken(ene);
            }
            if (!ene.CheckEnemyType(EnemyType.ImmuneToArmorBreaking) && bulletType.HasFlag(StatusEffectType.ArmorBreaking))
            {
                fxManager.DisableArmor(ene);
            }
            if (bulletType.HasFlag(StatusEffectType.ArmorDestroyer))
            {
                if (ene.enemyState.HasFlag(EnemyState.Armored))
                {
                    ene.ArmorBreak();
                }
                ene.DisableState(EnemyState.Armored);
                ene.armorStat.ArmorBarAdjust(false);
            }
        }
    }
    float CritDamage()
    {
        return bulletDamage.value * critDamage.value;
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
