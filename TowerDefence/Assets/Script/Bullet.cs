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
        EnemyType enemyType;
        ene.EnemyType(out enemyType);
        float Modifier = BaseTurretStat.CheckType(turretType, enemyType);//get rid of
        float damage;
        if (ene != null)
        {
            if (Random.value < critChance.baseValue)
            {
                damage = CritDamage() * Modifier;
                ene.TakeDamage(damage, DamageDisplayerType.Critial);
            }
            else
            {
                damage = bulletDamage.baseValue * Modifier;
                ene.TakeDamage(damage);
            }
            /*if (enemyType.HasFlag(EnemyType.ImmunityToAll))
            {
                return;
            }*/
            if (bulletType.HasFlag(BulletType.Insta_Kill))
            {
                effectManager.Insta_kill(ene);
                //ene.Insta_kill(insta_killptc.baseValue);
                //Debug.Log("Insta_kill");
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
        }
    }
    void DisplayDamage(float damage, Transform pos)
    {
        GameObject damdis = Instantiate(damageDisplayUI, pos.transform.position, target.rotation);
        damdis.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().GetComponent<TMP_TextController>().Blushing();
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
