using UnityEngine;

[System.Flags]
public enum StatusEffectType
{
    None = 0,
    Explode = 1 << 0,
    SlowPerSecond = 1 << 1,
    Dots = 1 << 3,
    Fear = 1 << 4,
    Insta_Kill = 1 << 5,
    Weaken = 1 << 6,
    ArmorBreaking = 1 << 7,
    PiercingShot = 1 << 8,
    ArmorDestroyer = 1<<9,
    Cast = 1 << 10,
    JustStoodStill = 1 << 12,
    RemoveAll = ~(-1 << 13)
}

public class BaseBulletClass : MonoBehaviour
{
    public Transform target;
    public CharacterStat bulletSpeed;
    public CharacterStat bulletDamage;
    public CharacterStat critChance;
    public CharacterStat critDamage;
    public GameObject ImpactFx;
    public StatusEffectType bulletType;

    protected AudioSource audioSource;


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
