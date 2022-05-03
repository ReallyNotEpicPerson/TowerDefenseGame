using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour
{
    private Transform target;
    private Transform spawnPoint;
    NavMeshAgent agent;
    private Transform OGPosition;
    [SerializeField] private CharacterStat tempSpeed;
    [SerializeField] private CharacterStat tempAcceleration;
    private float OGspeed;
    private float distanceRemain;
    [SerializeField] private float rateOfDistanceUpdate=0.2f;
    private float rate;
    public void OnValidate()
    {
        /*if(target == null || SpawnPoint == null)
        {
            Debug.LogError("No");
        }*/
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        tempSpeed.baseValue = agent.speed;
        tempAcceleration.baseValue = agent.acceleration;
        rate = rateOfDistanceUpdate;
        //agent.SetDestination(TheSpawner.endPoint.position);
        //target = TheSpawner.endPoint;
    }
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        OGspeed = agent.speed;
        OGPosition = target;
    }
    void Update()
    {
        if (OGPosition.position != target.position)
        {
            OGPosition = target;
            agent.SetDestination(target.position);
        }
        if (agent.isActiveAndEnabled)
        {
            CheckDestinationReached();
        }
        if (rate <= 0)
        {
            distanceRemain = PathRemainingDistance();
            rate = rateOfDistanceUpdate;
        }
        rate-=Time.deltaTime;
    }
    #region SetDestination
    public void TurnBack(int i)
    {
        if (i == 0)
        {
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(spawnPoint.position);
            }
        }
        else if (i == 1)
        {
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(target.position);
            }
        }
    }
    public void SetDestination(Transform SpawnPoint, Transform newTarget)
    {
        spawnPoint = SpawnPoint;
        target = newTarget;
        agent.SetDestination(target.position);
    }
    public void SetDestination()
    {
        agent.SetDestination(target.position);
    }
    #endregion
    public void SetSpeed(float s)
    {
        agent.speed = s;
    }
    public void SetSpeed()
    {
        agent.speed = OGspeed;
    }
    public void AddAccelerationMod(StatModifier acceleration)
    {
        tempAcceleration.AddModifier(acceleration);
        agent.acceleration = tempAcceleration.value;
    }
    public void AddSpeedMod(StatModifier slowPtc)
    {
        //Debug.Log("temp speed :" + tempSpeed.value);
        tempSpeed.AddModifier(slowPtc);
        //Debug.Log("speed mod added :" + tempSpeed.value);
        agent.speed = tempSpeed.value;
    }
    public void Enable(bool b)
    {
        agent.enabled = b;
    }
    public void RemoveMod(object source)
    {
        tempSpeed.RemoveAllModifiersFromSource(source);
        // Debug.Log("speed mod removed " + speed.value);
        agent.speed = tempSpeed.value;
    }
    public float GetDistanceFromSpawnPoint()
    {
        float ok= Vector3.SqrMagnitude(spawnPoint.position-transform.position);
        Debug.Log(ok);
        return ok;
    }
    public float GetDistanceRemain()
    {
        return distanceRemain;
    }
    public float PathRemainingDistance()
    {
        //Debug.Log(agent.path.corners[0]+ " " + agent.path.corners[1]);
        if (agent.pathPending ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            agent.path.corners.Length == 0)
            return -1f;

        float distance = 0.0f;
        for (int i = 0; i < agent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.SqrMagnitude(agent.path.corners[i]- agent.path.corners[i + 1]);
        }
        return distance;
    }
    void CheckDestinationReached()
    {
        //Debug.Log(name+" "+ agent.remainingDistance);
        /*if (Vector3.SqrMagnitude(transform.position - target.position) <= 0.1f)
        {
            print("Destination reached");
            gameObject.SetActive(false);
            EndPath();
        }*/
        if (agent.remainingDistance <= 0.1f)
        {
            print("Destination reached");
            gameObject.SetActive(false);
            EndPath();
        }
    }
    void EndPath()
    {
        PlayerStat.Lives--;
        Destroy(gameObject);
        TheSpawner.numOfEnemies--;
    }
}
