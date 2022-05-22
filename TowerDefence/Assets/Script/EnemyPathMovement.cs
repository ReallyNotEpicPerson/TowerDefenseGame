using UnityEngine;

//[RequireComponent(typeof(Enemy))]
public class EnemyPathMovement : MonoBehaviour
{
    private Transform Target;
    [Range(0, 3)]
    public int pathIndex = 0;
    private int wpIndex = 0;
    private bool turnBack = false;
    public CharacterStat startSpeed;
    public float distanceChangingWayPoint = 0.1f;
    [HideInInspector]
    public float speed;
    private float distanceRemain;
    public float rateOfUpdateDistance = 0.2f;
    private float rate;
    private AudioSource audioSource;

    public void Start()
    {
        TryGetComponent(out audioSource);
        if (pathIndex == 0)
        {
            Target = WayPoint.Points[0];
        }
        else if (pathIndex == 1)
        {
            Target = WayPoint.Points2[0];
        }
        else if (pathIndex == 2)
        {
            Target = WayPoint.Points3[0];
        }
        /*else if (pathIndex == 3)
        {
            Target = WayPoint.Points4[0];
        }*/
        speed = startSpeed.value;
    }
    void Update()
    {
        if (pathIndex == 0)
        {
            Vector3 dir = Target.position - transform.position;
            transform.Translate(speed * Time.deltaTime * dir.normalized, Space.World);
            //DistantCovered += Vector3.Distance()

            if (turnBack == false)
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    NextWayPoint_0();
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    PreviousWayPoint_0();
                }
            }
            if (rate <= 0)
            {
                distanceRemain = PathRemainingDistanceFirstPath();
                rate = rateOfUpdateDistance;
            }
            rate -= Time.deltaTime;
        }
        else if (pathIndex == 1)
        {
            Vector3 dir = Target.position - transform.position;
            transform.Translate(speed * Time.deltaTime * dir.normalized, Space.World);
            //DistantCovered += Vector3.Distance()

            if (turnBack == false)
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    NextWayPoint_1();
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    PreviousWayPoint_1();
                }
            }
            if (rate <= 0)
            {
                distanceRemain = PathRemainingDistanceSecondPath();
                rate = rateOfUpdateDistance;
            }
            rate -= Time.deltaTime;
        }
        else if (pathIndex == 2)
        {
            Vector3 dir = Target.position - transform.position;
            transform.Translate(speed * Time.deltaTime * dir.normalized, Space.World);
            //DistantCovered += Vector3.Distance()

            if (turnBack == false)
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    NextWayPoint_2();
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, Target.position) <= distanceChangingWayPoint)
                {
                    PreviousWayPoint_2();
                }
            }
            if (rate <= 0)
            {
                distanceRemain = PathRemainingDistanceThirdPath();
                rate = rateOfUpdateDistance;
            }
            rate -= Time.deltaTime;
        }
    }
    public void SetPathIndex(int i)
    {
        pathIndex = i;
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
    public void Turn_0()
    {
        turnBack = !turnBack;
        if (turnBack == false)
        {
            if (pathIndex == 0)
            {
                NextWayPoint_0();
            }
            else if (pathIndex == 1)
            {
                NextWayPoint_1();
            }
            else if (pathIndex == 2)
            {
                NextWayPoint_2();
            }
        }
        else
        {
            if (pathIndex == 0)
            {
                PreviousWayPoint_0();
            }
            else if (pathIndex == 1)
            {
                PreviousWayPoint_1();
            }
            else if (pathIndex == 2)
            {
                PreviousWayPoint_2();
            }
        }
    }
    private void NextWayPoint_0()
    {
        if (wpIndex >= WayPoint.Points.Length - 1)
        {
            EndPath();
            return;
        }
        wpIndex++;
        Target = WayPoint.Points[wpIndex];
    }
    private void NextWayPoint_1()
    {
        if (wpIndex >= WayPoint.Points2.Length - 1)
        {
            EndPath();
            return;
        }
        wpIndex++;
        Target = WayPoint.Points2[wpIndex];
    }
    private void NextWayPoint_2()
    {
        if (wpIndex >= WayPoint.Points3.Length - 1)
        {
            EndPath();
            return;
        }
        wpIndex++;
        Target = WayPoint.Points3[wpIndex];
    }
    private void PreviousWayPoint_0()
    {
        if (wpIndex == 0)
        {
            //EndPath();
            return;
        }
        wpIndex--;
        Target = WayPoint.Points[wpIndex];
    }
    private void PreviousWayPoint_1()
    {
        if (wpIndex == 0)
        {
            //EndPath();
            return;
        }
        wpIndex--;
        Target = WayPoint.Points2[wpIndex];
    }
    private void PreviousWayPoint_2()
    {
        if (wpIndex == 0)
        {
            //EndPath();
            return;
        }
        wpIndex--;
        Target = WayPoint.Points3[wpIndex];
    }
    void EndPath()
    {
        Debug.Log("THEY PASSSSS through Dumbass");
        GameAsset.I.audioSource.PlayOneShot(GameAsset.I.whenEnemyPassThrough);
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
        return Vector3.SqrMagnitude(WayPoint.Points[0].position - transform.position);
    }
    public float PathRemainingDistanceFirstPath()
    {
        float distance = 0.0f;
        for (int i = wpIndex; i < WayPoint.Points.Length - 2; ++i)
        {
            if (i == wpIndex)
            {
                distance += Vector3.SqrMagnitude(transform.position - WayPoint.Points[i].position);
                continue;
            }
            distance += Vector3.SqrMagnitude(WayPoint.Points[i].position - WayPoint.Points[i + 1].position);
        }
        return distance;
    }
    public float PathRemainingDistanceSecondPath()
    {
        float distance = 0.0f;
        for (int i = wpIndex; i < WayPoint.Points2.Length - 2; ++i)
        {
            if (i == wpIndex)
            {
                distance += Vector3.SqrMagnitude(transform.position - WayPoint.Points2[i].position);
                continue;
            }
            distance += Vector3.SqrMagnitude(WayPoint.Points2[i].position - WayPoint.Points2[i + 1].position);
        }
        return distance;
    }
    public float PathRemainingDistanceThirdPath()
    {
        float distance = 0.0f;
        for (int i = wpIndex; i < WayPoint.Points3.Length - 2; ++i)
        {
            if (i == wpIndex)
            {
                distance += Vector3.SqrMagnitude(transform.position - WayPoint.Points3[i].position);
                continue;
            }
            distance += Vector3.SqrMagnitude(WayPoint.Points3[i].position - WayPoint.Points3[i + 1].position);
        }
        return distance;
    }
}
