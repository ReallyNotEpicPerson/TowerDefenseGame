using UnityEngine;

[System.Flags]
public enum BulletType
{
    None = 0,
    Explode = 1<<0,//done?
    Slow = 1<<1,//done
    SlowPerSecond = 1<<2,//done
    Mine = 1<<3,//soon
    Burn = 1<<4,//done
    Fear = 1<<5,//done
    Insta_Kill = 1<<6,//done
    Weaken = 1<<7,
}
public class BaseBulletClass : MonoBehaviour
{
    public Transform target;

    public CharacterStat speed;
    public CharacterStat bulletDamage;
    public CharacterStat critChance;
    public CharacterStat critDamage;

    public GameObject ImpactFx;

    [SerializeField] protected string Taggu = "Enemy";

    public BulletType bulletType;
    //public PassiveAbility passiveAbility;
}
