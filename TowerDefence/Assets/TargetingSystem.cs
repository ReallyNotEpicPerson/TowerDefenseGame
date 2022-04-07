using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : BaseTurretStat
{
    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("Final "+nearestCol.name);
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
    }
}
