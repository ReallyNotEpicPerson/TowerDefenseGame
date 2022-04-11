using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ShootType
{
    None=0,
    SingleTarget =1,
    MultipleTarget = 2,
}
[System.Flags]
public enum PassiveAbility
{
    None=0,
    Penetration = 1<<0,
    Splash = 1<<1,
    IncreaseDamage = 1<<2,
    IncreaseSpeed = 1<<3,
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
    public PassiveAbility passiveAbility;
    public int numberOfTarget=1;
    public CharacterStat range;//keep at all cost
    public CharacterStat rotationSpeed;//keep at all cost
    [SerializeField] protected List<Transform> target;//keep at all cost

    protected Direction direction;

    public virtual void Awake()
    {
        target = new List<Transform>(numberOfTarget);
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

}/*
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