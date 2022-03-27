using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour
{
    public Transform target;
    public Transform SpawnPoint;
    NavMeshAgent agent;
    private Transform OGPosition;
    private float originalSpeed;
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
        agent.SetDestination(target.position);
    }
    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        OGPosition = target;
        originalSpeed = agent.speed;
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
    }
    #region SetDestination
    public void TurnBack(int i)
    {
        if (i==0)
        {
            agent.SetDestination(SpawnPoint.position);
        }
        else if(i==1)
        {
            agent.SetDestination(target.position);
        }
    }

    public void SetDestination(Transform newLoc)
    {
        agent.SetDestination(newLoc.position);
    }
    public void SetDestination(Vector3 newLoc)
    {
        agent.SetDestination(newLoc);
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
    public void RemoveMod(object source)
    {
        tempSpeed.RemoveAllModifiersFromSource(source); 
       // Debug.Log("speed mod removed " + speed.value);
        agent.speed = tempSpeed.value;
    }
    public void RestoreSpeed()
    {
        //speed.RemoveAllModifiersFromSource(this);
        //Debug.Log(speed);
        agent.speed = originalSpeed;
    }
}
