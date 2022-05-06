using UnityEngine;

[System.Flags]
public enum BulletType
{
    None = 0,
    Explode = 1 << 0,//done?
    SlowPerSecond = 1 << 1,//done
    Mine = 1 << 2,//soon
    Dots = 1 << 3,//done
    Fear = 1 << 4,//done
    Insta_Kill = 1 << 5,//done
    Weaken = 1 << 6,//Done
    ArmorBreaking = 1 << 7,//done
    PiercingShot = 1 << 8,//done
    Cast = 1 << 9,
    
    
    JustStoodStill = 1 << 12,//IDK man, dont delete
}

public class BaseBulletClass : MonoBehaviour
{
    public Transform target;
    public CharacterStat bulletSpeed;
    public CharacterStat bulletDamage;
    public CharacterStat critChance;
    public CharacterStat critDamage;
    public GameObject ImpactFx;

   // [SerializeField] protected string taggu = "Enemy";
    [SerializeField] protected string specialTag = "Invisible";

    public BulletType bulletType;
    //public PassiveAbility passiveAbility;
    public void AddDamageMod(StatModifier mod)
    {
        bulletDamage.AddingOneInstance(mod);
    }
    public void UndoDamageMod(object source)
    {
        bulletDamage.RemoveAllModifiersFromSource(source);
    }
}
