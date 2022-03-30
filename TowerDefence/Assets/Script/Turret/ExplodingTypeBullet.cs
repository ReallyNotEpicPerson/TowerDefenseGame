using TMPro;
using UnityEditor;
using UnityEngine;

public class ExplodingTypeBullet : BaseBulletClass
{//THIS script will be gone soon.....
    [SerializeField] private CharacterStat[] additionalDamage;
    [SerializeField] private CharacterStat explosionRadius;
    [SerializeField] private CharacterStat slowDownSecond ;//PerSecondSlow
    [SerializeField] private CharacterStat slowPtc;//slowing down ptc
    [SerializeField] private CharacterStat dotsTime;
    [SerializeField] private CharacterStat dots;

    public void Seek(Transform _target)
    {
        target = _target;
    }/*
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
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
    }*/
    void HitTarget()
    {
        GameObject Fx = Instantiate(ImpactFx, transform.position, target.rotation);
        Destroy(Fx, 1.5f);

        if (bulletType.HasFlag(BulletType.Explode))
        {    
            //Explode();
        }
        else
        {
            Debug.LogError("ForgetSth? explosion type?");
            return;
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
               // Damage(col.transform);
                
            }
        }
    }
    void Damage(Transform enemy)
    {
        
    }
    float CritDamage()
    {
        return bulletDamage.baseValue * critDamage.baseValue;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), explosionRadius.baseValue);
        //Gizmos.DrawWireSphere(transform.position,explosionRadius);    
    }
#endif
}

