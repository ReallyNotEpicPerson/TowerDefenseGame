using System;
using UnityEngine;

//[RequireComponent(typeof(Enemy))]
public class EnemyPathMovement : MonoBehaviour
{
    private Transform Target;
    private int wpIndex = 0;
    private bool turnBack = false;
    public CharacterStat startSpeed;
    [HideInInspector]
    public float speed;
    private float distanceRemain;
    public float rateOfUpdateDistance=0.2f;
    private float rate;

    private void Awake()
    {
        //enemy = GetComponent<Enemy>();
        Target = WayPoint.Points[0];
    }
    private void Start()
    {
        speed = startSpeed.value;
    }
    void Update()
    {
        //Target.position.z *= 0;
        Vector3 dir = Target.position - transform.position;
        transform.Translate(speed * Time.deltaTime * dir.normalized, Space.World);
        //DistantCovered += Vector3.Distance()

        if (turnBack == false)
        {
            if (Vector3.Distance(transform.position, Target.position) <= 0.1f)
            {
                NextWayPoint();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, Target.position) <= 0.1f)
            {
                PreviousWayPoint();
            }
        }
        if (rate <= 0)
        {        
            distanceRemain = PathRemainingDistance();
            rate = rateOfUpdateDistance;
        }
        rate -= Time.deltaTime;
    }
    public void SetSpeed(float s)
    {
        speed = s;
    }
    public void AddSpeedMod(StatModifier mod)
    {
        startSpeed.AddModifier(mod);
        speed = startSpeed.value;
    }
    public void RemoveMod(object source)
    {
        startSpeed.RemoveAllModifiersFromSource(source);
        speed = startSpeed.value;
    }
    public void Turn()
    {
        turnBack = !turnBack;
        if (turnBack == false)
        {
            NextWayPoint();
        }
        else
        {
            PreviousWayPoint();
        }
    }
    private void NextWayPoint()
    {
        if (wpIndex >= WayPoint.Points.Length - 1)
        {
            EndPath();
            return;
        }
        wpIndex++;
        Target = WayPoint.Points[wpIndex];
    }
    private void PreviousWayPoint()
    {
        if (wpIndex == 0)
        {
            //EndPath();
            return;
        }
        wpIndex--;
        Target = WayPoint.Points[wpIndex];
    }
    void EndPath()
    {
        PlayerStat.Lives--;
        Destroy(gameObject);
        TheSpawner.numOfEnemies--;
    }
    public float GetDistanceRemain()
    {
        return distanceRemain;
    }
    public float GetDistanceFromSpawnPoint()
    {
        return Vector3.SqrMagnitude(WayPoint.Points[0].position-transform.position);
    }
    public float PathRemainingDistance()
    {
        float distance = 0.0f;
        for (int i = wpIndex; i < WayPoint.Points.Length - 2; ++i)
        {
            if (i == wpIndex)
            {
                distance += Vector3.SqrMagnitude(transform.position- WayPoint.Points[i].position);
                continue;
            }
            distance += Vector3.SqrMagnitude(WayPoint.Points[i].position - WayPoint.Points[i+1].position);
        }
        return distance;
    }
}
