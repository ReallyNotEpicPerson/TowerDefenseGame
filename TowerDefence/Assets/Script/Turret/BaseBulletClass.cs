using UnityEngine;

[System.Flags]
public enum BulletType
{
    None = 0,
    Explode = 1<<0,//done?
    Slow = 1<<1,//done
    SlowPerSecond = 1<<2,//done
    Sticky = 1<<3,//not now
    Mine = 1<<4,//soon
    Burn = 1<<5,//done
    Fear = 1<<6,//done
    Insta_Kill = 1<<7,//done
}
public class BaseBulletClass : MonoBehaviour
{
    public Transform target;

    public CharacterStat speed;
    public CharacterStat bulletDamage;
    public CharacterStat critChance;
    public CharacterStat critDamage;

    public GameObject damageDisplayUI;
    public GameObject ImpactFx;

    [SerializeField] protected string Taggu = "Enemy";

    public BulletType bulletType;
    //public PassiveAbility passiveAbility;

    private void OnValidate()
    {
        if (damageDisplayUI == null)
        {
            damageDisplayUI = (GameObject)Resources.Load("Displayer");
        }
    }

}
