#if UNITY_EDITOR
#endif
using UnityEngine;

public class Bullet : BaseBulletClass
{
    public GameObject fxManager;
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private CharacterStat explosionRadius;
    float finalDamage;
    public void OnValidate()//taggu
    {
        if (effectManager == null)
        {
            effectManager = fxManager.GetComponentInChildren<EffectManager>();
        }
        if (fxManager == null || effectManager == null)
        {
            Debug.Log(name);
        }
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
    void Update()//make bullet move and shit
    {       
        if (target == null)
        {
            Destroy(gameObject);
            //Bullet.SetActive(false);
            return;
        }
        if (bulletType.HasFlag(BulletType.JustStoodStill))
        {
            return;
        }
        Vector3 dir = target.position - transform.position;
        float DistantThisFrame = bulletSpeed.baseValue * Time.deltaTime;
        if (dir.magnitude <= DistantThisFrame)
        {
            //Debug.Log("hit now");
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * DistantThisFrame, Space.World);
        //transform.LookAt(target);
    }
    void HitTarget()
    {
        GameObject Fx = Instantiate(ImpactFx, transform.position, target.rotation);
        Destroy(Fx, 1f);
        if (bulletType.HasFlag(BulletType.Explode))
        {
            Explode();
        }
        else
        {
            if (bulletType.HasFlag(BulletType.Cast))
            {
                EnemyCast(target.GetComponent<Enemy>());
            }
            else
                Damage(target.GetComponent<Enemy>());
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
                if (bulletType.HasFlag(BulletType.Cast))
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
        if (bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            effectManager.Slow(ene);
        }
        if (bulletType.HasFlag(BulletType.Dots))
        {
            effectManager.Dots(ene);
        }
    }
    void Damage(Enemy ene)
    {
        //Debug.Log("Mod num " + bulletDamage.StatModifiers.Count + " Damage after mod " + bulletDamage.value);
        //Enemy ene = enemy.GetComponent<Enemy>();
        if (Random.value <= ene.ChanceToEvade)
        {
            /*if (!ene.enemyState.HasFlag(EnemyState.FirstHit))
            {
                ene.EnableState(EnemyState.FirstHit);
            }*/
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

                if (bulletType.HasFlag(BulletType.PiercingShot))
                {
                    ene.ArmorPiercing(finalDamage, DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(finalDamage, DamageDisplayerType.Critial);
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
                if (bulletType.HasFlag(BulletType.PiercingShot))
                {
                    ene.ArmorPiercing(finalDamage, DamageDisplayerType.ArmorPenetration);
                }
                else
                {
                    ene.TakeDamage(finalDamage);
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
            if (!ene.CheckEnemyType(EnemyType.ImmunityToArmorBreaking) && bulletType.HasFlag(BulletType.ArmorBreaking))
            {
                effectManager.DisableArmor(ene);
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
