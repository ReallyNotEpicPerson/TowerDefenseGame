using UnityEngine;

[System.Flags]
public enum BulletType
{
    None = 0,
    Explode = 1<<0,//done?
    SlowPerSecond = 1<<1,//done
    Mine = 1<<2,//soon
    Dots = 1<<3,//done
    Fear = 1<<4,//done
    Insta_Kill = 1<<5,//done
    Weaken = 1<<6,//Done
    DisableArmor = 1<<7,//ye
    ArmorPiercing = 1<<8,//maybe
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
