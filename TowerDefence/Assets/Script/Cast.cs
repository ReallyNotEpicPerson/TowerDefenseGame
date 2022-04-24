using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Cast : MonoBehaviour
{
    public ShootType castType;
    protected List<Transform> target;
    public int numberOfTarget = 1;
    public float castRange;
    public float castRate;
    public float delay;
    public float repeatRate = 0.2f;
    public Transform[] castPoint;
    public GameObject healingBulletPrefab;
    
    private float timer;

    private void Awake()
    {
        target = new List<Transform>();
        timer = 1 / castRate;
        //ok.sam
    }

    public void FindTarget()
    {
        InvokeRepeating(nameof(UpdateTarget), delay, repeatRate);
    }
    public void SpawnEnemy()
    {
        InvokeRepeating(nameof(Necromancy), delay, repeatRate);
    }
    public void NoMoreNecromancy()
    {
        CancelInvoke(nameof(SpawnEnemy));
    }
    public void NoMoreFindTarget()
    {
        CancelInvoke(nameof(SpawnEnemy));
        target.Clear();
    }
    private void Necromancy()
    {
        int i = 0;
        while (i<numberOfTarget)
        {
            if (NavMesh.SamplePosition(transform.position,out NavMeshHit hit,castRange,NavMesh.AllAreas))//transform.position + GetRandomPos())
            {
                Debug.Log(hit.position);
                GameObject underling = Instantiate(GameAsset.I.underling, hit.position,Quaternion.identity);
                underling.TryGetComponent(out NavMeshAI navMesh);
                navMesh.SetDestination(TheSpawner.spawnPoint[0], TheSpawner.endPoint[0]);
                i++;
            }           
        }
        /*for (int i = 0; i < numberOfTarget; i++)
        {*/
            
        //}
    }
    void UpdateTarget()
    {
        //Debug.Log("YEP IM CUMMING");
        target.Clear();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, castRange);
        float shortestDis = Mathf.Infinity;
        Collider2D nearestCol = null;
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent<Enemy>(out _))
            {
                if (castType.HasFlag(ShootType.SingleTarget))
                {
                    float DisToenenmy = Vector3.Distance(transform.position, col.transform.position);//use Distancesquared??
                    if (DisToenenmy < shortestDis)
                    {
                        shortestDis = DisToenenmy;
                        nearestCol = col;
                    }
                }
                else if (castType.HasFlag(ShootType.MultipleTarget))
                {
                    target.Add(col.transform);
                    if (target.Count == numberOfTarget)
                    {
                        return;
                    }
                }
            }
        }
        if (castType.HasFlag(ShootType.SingleTarget) && nearestCol != null)
        {
            target.Add(nearestCol.transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target.Count == 0)
        {
            return;
        }
        if (timer <= 0f)
        {
            timer = 1 / castRate;
            Shoot();
        }
        timer -= Time.deltaTime;
    }
    public void Shoot()
    {
        Debug.Log("Shoot");
        for (int i = 0; i < castPoint.Length; i++)
        {
            for (int j = 0; j < target.Count; j++)
            {
                Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.Seek(target[j]);
                }
            }
        }
    }

    public Vector3 GetRandomPos()
    {
        return Random.insideUnitCircle * Random.Range(-castRange, castRange);
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(healingBulletPrefab, castPoint[i].position, castPoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), castRange);
    }
#endif
}
