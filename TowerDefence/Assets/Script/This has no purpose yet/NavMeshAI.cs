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
        //agent.SetDestination(TheSpawner.endPoint.position);
        //target = TheSpawner.endPoint;
        //spawnPoint = 
    }
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        OGPosition = target;
        tempSpeed.baseValue = agent.speed;
        tempAcceleration.baseValue = agent.acceleration;
    }
    void Update()
    {
        if (OGPosition.position != target.position)
        {
            OGPosition = target;
            agent.SetDestination(target.position);
        }
        CheckDestinationReached();        
    }
    #region SetDestination
    public void TurnBack(int i)
    {
        if (i == 0)
        {
            agent.SetDestination(spawnPoint.position);
        }
        else if (i == 1)
        {
            if (isActiveAndEnabled==false) { return; }
            agent.SetDestination(target.position);
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

    void CheckDestinationReached()
    {
        //Debug.Log(Vector3.SqrMagnitude(transform.position - target.position));
        if (Vector3.SqrMagnitude(transform.position - target.position) <= 0.1f)
        {
            print("Destination reached");
            gameObject.SetActive(false);
            EndPath();
        }
        /*if (agent.remainingDistance <= 0.1f)
        {
            print("Destination reached");
            gameObject.SetActive(false);
            EndPath();
        }*/
    }
    void EndPath()
    {
        PlayerStat.Lives--;
        Destroy(gameObject);
        TheSpawner.numOfEnemies--;
    }
}
