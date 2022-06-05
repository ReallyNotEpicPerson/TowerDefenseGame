using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ShootType
{
    None = 0,
    SingleTarget = 1,
    MultipleTarget = 2,
    ArmoredTarget = 4,
}
public enum TargetingType
{
    None = 0,
    Closest = 1,
    First = 2,
    LeastHealth = 4,
    MostHealth = 8,
    Random = 16,
    Armored = 32,
}
[System.Flags]
public enum PassiveAbility
{
    None = 0,
    Penetration = 1,//nope
    Splash = 1 << 1,//nope
    IncreaseDamage = 1 << 2,//maybe
    IncreaseSpeed = 1 << 3,//maybe
    CanShootWhenBuy = 1 << 4,
    CanSeeInvisibleUnit = 1 << 5,
    QuadrupleDamage = 1 << 6,
    InfiniteRange = 1 << 7,
    RemoveAll = ~(-1 << 9)

}
public enum Direction
{
    Left,
    Right,
    Front,
    Back,
}
[System.Serializable]
public class BaseTurretStat : MonoBehaviour
{
    public ShootType shootType;
    public TargetingType targetingType;
    public PassiveAbility passiveAbility;
    public int numberOfTarget = 1;
    public CharacterStat range;//keep at all cost
    public CharacterStat rotationSpeed;//keep at all cost
    protected List<Transform> target;//keep at all cost
    public SpriteRenderer spriteRenderer;
    private float rangeTimer;
    //protected Direction direction;

    public virtual void Awake()
    {
        if (this is SupportTypeTurret)
        {
            target = new List<Transform>();
        }
        else { target = new List<Transform>(numberOfTarget); }
        TryGetComponent(out spriteRenderer);
    }
    public virtual void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }
    public virtual void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
    }
    public virtual void OnMouseEnter()
    {
        if (transform.childCount > 1)
            transform.GetChild(1).gameObject.SetActive(true);
        else
            transform.GetChild(0).gameObject.SetActive(true);
    }
    public virtual void OnMouseExit()
    {
        if (transform.childCount > 1)
            transform.GetChild(1).gameObject.SetActive(false);
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }

    public virtual void RotateToObject()
    {
        float angle = Mathf.Atan2(target[0].transform.position.y - transform.position.y, target[0].transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        //Debug.DrawRay(transform.position, target.transform.position);
    }
    public void DisableThePewPew()
    {
        if (passiveAbility.HasFlag(PassiveAbility.CanShootWhenBuy))
        {
            return;
        }
        if (TryGetComponent(out Collider2D col))
        {
            col.enabled = false;
        }
        enabled = false;
    }
    public void FadeAbout(float ptc)
    {
        spriteRenderer.Fade(ptc);
    }
    public virtual void UpdateTarget()
    {
        target.Clear();
        Collider2D[] collider;
        if (passiveAbility.HasFlag(PassiveAbility.InfiniteRange))
        {
            collider = Physics2D.OverlapCircleAll(transform.position, 50);
        }
        else
        {
            collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        }
        float shortestDistance = Mathf.Infinity;
        float pathCovered = Mathf.Infinity;
        float leasthealth = Mathf.Infinity;
        float mostHealth = Mathf.NegativeInfinity;
        Collider2D ChosenCol = null;
        List<Transform> ChosenCols = new List<Transform>(numberOfTarget);
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Enemy enemy))
            {
                if (enemy.enemyState.HasFlag(EnemyState.Invisible) && !passiveAbility.HasFlag(PassiveAbility.CanSeeInvisibleUnit))
                {
                    continue;
                }
                if (shootType.HasFlag(ShootType.SingleTarget))
                {
                    if (targetingType.HasFlag(TargetingType.Closest))
                    {
                        float DisToenenmy = Vector3.SqrMagnitude(transform.position - col.transform.position);//use Distancesquared??
                        if (DisToenenmy < shortestDistance)
                        {
                            shortestDistance = DisToenenmy;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.First))
                    {
                        float p = enemy.RemainingPath();
                        // Debug.Log("path " + p);
                        if (p < pathCovered)
                        {
                            pathCovered = p;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.LeastHealth))
                    {
                        float h = enemy.GetHealthAmount();
                        if (h < leasthealth)
                        {
                            leasthealth = h;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.MostHealth))
                    {
                        float h = enemy.GetHealthAmount();
                        if (h > mostHealth)
                        {
                            mostHealth = h;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.Random))
                    {
                        ChosenCol = collider[Random.Range(0, collider.Length - 1)];
                        break;
                    }
                }
                else if (shootType.HasFlag(ShootType.MultipleTarget))
                {
                    /*if (targetingType.HasFlag(TargetingType.Random))
                    {*/
                    target.Add(col.transform);
                    if (target.Count == numberOfTarget)
                    {
                        return;
                    }
                    //}
                    /*else if (targetingType.HasFlag(TargetingType.Closest))
                    {
                        float DisToenenmy = Vector3.SqrMagnitude(transform.position - col.transform.position);//use Distancesquared??
                        if (DisToenenmy < shortestDistance)
                        {
                            shortestDistance = DisToenenmy;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.First))
                    {
                        float p = enemy.RemainingPath();
                        // Debug.Log("path " + p);
                        if (p < pathCovered)
                        {
                            pathCovered = p;
                            if (target[0] == null)
                            {
                                target[0] = col.transform;
                            }
                            else
                            {
                                for (int i = 1; i < target.Count; i++)
                                {
                                    target[i] = target[i-1];
                                }
                                target[0] = col.transform;
                            }
                        }
                    }*/
                }
                else if (targetingType.HasFlag(TargetingType.Armored))
                {
                    if (enemy.enemyState.HasFlag(EnemyState.Armored))
                    {
                        ChosenCol = col;
                        break;
                    }
                    if (targetingType.HasFlag(TargetingType.Closest))
                    {
                        float DisToenenmy = Vector3.SqrMagnitude(transform.position - col.transform.position);//use Distancesquared??
                        if (DisToenenmy < shortestDistance)
                        {
                            shortestDistance = DisToenenmy;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.First))
                    {
                        float p = enemy.RemainingPath();
                        // Debug.Log("path " + p);
                        if (p < pathCovered)
                        {
                            pathCovered = p;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.LeastHealth))
                    {
                        float h = enemy.GetHealthAmount();
                        if (h < leasthealth)
                        {
                            leasthealth = h;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.MostHealth))
                    {
                        float h = enemy.GetHealthAmount();
                        if (h > mostHealth)
                        {
                            mostHealth = h;
                            ChosenCol = col;
                        }
                    }
                    else if (targetingType.HasFlag(TargetingType.Random))
                    {
                        ChosenCol = collider[Random.Range(0, collider.Length - 1)];
                        break;
                    }
                }
            }
        }
        if ((shootType.HasFlag(ShootType.SingleTarget) || targetingType.HasFlag(TargetingType.Armored)) && ChosenCol != null)
        {
            target.Add(ChosenCol.transform);
        }
        /*else if (shootType.HasFlag(ShootType.MultipleTarget))
        {
            target = ChosenCols;
        }*/
    }
}

/*
public struct Name : IEquatable<Name>
{
    private TurretType _turretType;
    private EnemyType _enemyType;
    public Name(TurretType t, EnemyType e)
    {
        _turretType = t;
        _enemyType = e;
    }
    public EnemyType enemyType
    {
        get { return _enemyType; }
        set { _enemyType = value; }
    }
    public TurretType turretType
    {
        get { return _turretType; }
        set { _turretType = value; }
    }
    public override bool Equals(object obj) => obj is Name other && Equals(other);

    public bool Equals(Name other)
    {
        return enemyType == other.enemyType &&
               turretType == other.turretType;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166146429;
            hash = (hash * 486194873) ^ enemyType.GetHashCode();
            hash = (hash * 486194873) ^ turretType.GetHashCode();
            return hash;
        }
    }
    public Name GetName()
    {
        return this;
    }
    public void DisableTurretTypeState(TurretType t)
    {
        turretType &= ~t;
    }
    public void DisableEnemyTypeState(EnemyType e)
    {
        enemyType &= ~e;
    }
    public void EnableTurretTypeState(TurretType t)
    {
        turretType |= t;
    }
    public void EnableEnemyTypeState(EnemyType e)
    {
        enemyType |= e;
    }
}

public struct TurretEntity
{
    CharacterStat Range;
    CharacterStat Target;
    
}*/