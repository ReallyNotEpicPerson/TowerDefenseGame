#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public List<Transform> firePoint;//keep at all cost
    public CharacterStat firerate;
    public GameObject BulletPrefab;
    public string etag = "Enemy";
    private float FireCountDown = 0f;
    //private ObjectPooler pool;
    /*private void Awake()
    {
        //pool = GetComponent<ObjectPooler>();
    }*/
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
    }
    void Start()
    {
        //InvokeRepeating("UpdateTarget", 0f, 0.2f);
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }

    void UpdateTarget()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position,range.value);
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
        }
        else
        {
            target = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        RotateToObject();
        if (FireCountDown <= 0f)
        {
            FireCountDown = 1f / firerate.baseValue;
            Shoot();
        }
        FireCountDown -= Time.deltaTime;
    }
    public void Shoot()
    {
        for (int i = 0; i < firePoint.Count; i++)
        {
            Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
            if (bullet != null) { bullet.Seek(target); }
        }
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
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