using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[System.Flags]
public enum PassiveAbility
{
    Stun = 1,
    ExtraShots = 2,
    Penetration = 4,
    Splash = 8,
    DamagePerSecond = 16,
    SpreadShot = 32,
    IncreaseDamage = 64,
    Insta_Kill = 128,
}
public enum ActiveAbility
{
    CallHitler,
}
public enum TurretType
{
    NormalBullet,
    CloseCombat,
    Group,
    Energy,
    Slow,
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
    public CharacterStat range;//keep at all cost
    public CharacterStat rotationSpeed;//keep at all cost

    public Transform target;//keep at all cost

    [SerializeField] protected TurretType turretType;
    protected Direction direction;
    private Node _node;

    public static Dictionary<Name, float> StatDict = new Dictionary<Name, float>
    {
        { new Name(TurretType.NormalBullet, EnemyType.None), 1 },
        { new Name(TurretType.NormalBullet, EnemyType.Fast),1.2f },
        { new Name(TurretType.NormalBullet, EnemyType.Tough),0.8f},
        { new Name(TurretType.Energy, EnemyType.None), 1},
        { new Name(TurretType.Energy, EnemyType.Fast),0.8f},
        { new Name(TurretType.Energy, EnemyType.Tough),1.2f},
        { new Name(TurretType.Group, EnemyType.None),0.8f},
        { new Name(TurretType.Group, EnemyType.Fast),1},
        { new Name(TurretType.Group, EnemyType.Tough),1.2f},
        { new Name(TurretType.CloseCombat,EnemyType.None),1.2f },
        { new Name(TurretType.CloseCombat,EnemyType.Fast),2f },
        { new Name(TurretType.CloseCombat,EnemyType.Tough),0.5f },
    };//template { new Name(TurretType.NormalBullet,EnemyType.Fast),1 }

    public void SetNode(Node node)
    {
        _node = node;
    }
    public static float CheckType(TurretType t, EnemyType e)
    {
        if (!StatDict.ContainsKey(new Name(t, e)))
        {
            Debug.Log(StatDict.ContainsKey(new Name(t, e)) + " Didn`t work ");
            return 1;
        }
        return StatDict[new Name(t, e)];
    }
    public virtual void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (_node != null)
        {
            _node.CallBuildManager();
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
        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        //Debug.DrawRay(transform.position, target.transform.position);
    }

    public static implicit operator BaseTurretStat(bool v)
    {
        throw new NotImplementedException();
    }
}
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
    
}

