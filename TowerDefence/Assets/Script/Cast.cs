using UnityEngine;

public enum CastType
{
    SingleTarget,
    MultipleTarget,
}
public class Cast : MonoBehaviour
{
    public string etag = "Enemy";
    public Transform[] target;
    public float castRange;
    public float repeatRate;
    private Transform[] castPoint;

    public void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, repeatRate);
    }
    void UpdateTarget()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, castRange);
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
        if (nearestCol != null && shortestDis <= castRange)
        {
            target[0] = nearestCol.transform;
            //Debug.Log("Final " + nearestCol.name);
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
    }
    /*public void Shoot()
    {      
        for (int i = 0; i < castPoint.Length; i++)
        {
            Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
            if (bullet != null) { bullet.Seek(target); }
        }
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }*/
}
