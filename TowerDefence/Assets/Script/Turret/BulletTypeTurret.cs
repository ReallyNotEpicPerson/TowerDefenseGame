#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public List<Transform> firePoint;//keep at all cost
    public CharacterStat fireRate;
    public GameObject BulletPrefab;
    private float FireCountDown = 0f;
    private EntityEffectHandler fxHandler;
    private EffectManager fxManager;
    private StatModifier modifier;

    //private ObjectPooler pool;
    public override void Awake()
    {
        base.Awake();
        TryGetComponent(out fxHandler);
        TryGetComponent(out fxManager);
    }
    public void OnValidate()
    {
        if (firePoint.Count < 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("FirePoint"))
                {
                    firePoint.Add(transform.GetChild(i));
                }
                else
                    continue;
            }
        }
        transform.GetChild(1).localScale=Vector3.one;
        transform.GetChild(1).localScale*=range.baseValue;
    }
    void Start()
    {
        //InvokeRepeating("UpdateTarget", 0f, 0.2f);
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
            return;
        }
        RotateToObject();
        if (FireCountDown <= 0f)
        {
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseSpeed))
            {
                fxHandler.AddDebuff(fxManager.GetSpeedBoostData(), gameObject);
            }
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
            {
                fxHandler.AddDebuff(fxManager.GetDamageBoostData(), gameObject);
            }
            FireCountDown = 1/fireRate.value;
            Shoot();
        }
        FireCountDown -= Time.deltaTime;
    }
    public void Shoot()
    {
        for (int i = 0; i < firePoint.Count; i++)
        {
            for (int j = 0; j < target.Count; j++)
            {
                Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
                if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
                {
                    bullet.AddDamageMod(modifier);
                }
                if (bullet != null)
                {
                    bullet.Seek(target[j]);
                }
            }
        }
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }
    public void ReduceRate(StatModifier mod)
    {
        fireRate.AddingOneInstance(mod);
        Debug.Log("Just added " + fireRate.value);
    }
    public void UndoModificationToFireRate(object source)
    {
        fireRate.RemoveAllModifiersFromSource(source);
        Debug.Log("Remove all " + fireRate.value);
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