#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public CharacterStat fireRate;
    [Header("List of Fire Point")]
    public List<Transform> firePoint;//keep at all cost

    private float FireCountDown = 0f;
    private EntityEffectHandler fxHandler;
    private EffectManager fxManager;
    private StatModifier modifier;

    [Header("Gameobject stuff")]
    public GameObject[] objectToRotate;
    [SerializeField] private Animator animator;
    public GameObject BulletPrefab;
    [SerializeField] private Bullet bu;
    private Dictionary<int, Dictionary<object, StatModifier>> bulletMod = new Dictionary<int, Dictionary<object, StatModifier>>();
    private AudioSource audioSource;

    public void OnValidate()
    {
        TryGetComponent(out fxManager);
        TryGetComponent(out fxHandler);
        if (bu == null)
        {
            BulletPrefab.TryGetComponent(out bu);
        }
        if (firePoint.Count < 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("FirePoint"))
                {
                    firePoint.Add(transform.GetChild(i));
                }
                else
                    continue;
            }
        }
        transform.Find("Range").localScale = Vector3.one;
        transform.Find("Range").localScale *= range.value;

    }
    public override void Awake()
    {
        base.Awake();
        /*if (bu == null)
        {
            BulletPrefab.TryGetComponent(out bu);
        }*/
        //bu.Seek(transform);
        //bu.bulletType |= BulletType.JustStoodStill;
        TryGetComponent(out fxHandler);
        TryGetComponent(out fxManager);
        TryGetComponent(out audioSource);
        bulletMod.Add(0, new Dictionary<object, StatModifier>());//Weaken value?
        bulletMod.Add(1, new Dictionary<object, StatModifier>());//Damage
        bulletMod.Add(2, new Dictionary<object, StatModifier>());//critdamage
        bulletMod.Add(3, new Dictionary<object, StatModifier>());//critchance
    }
    public override void Start()
    {
        base.Start();
        FireCountDown = 0;
    }
    void Update()
    {
        if (target.Count == 0 || target[0] == null)
        {
            return;
        }
        RotateToObject();
        if (FireCountDown <= 0f)
        {
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseSpeed))
            {
                fxHandler.AddDebuff(fxManager.GetSpeedBoostData(), gameObject);
            }
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
            {
                fxHandler.AddDebuff(fxManager.GetDamageBoostData(), gameObject);
            }
            FireCountDown = 1 / fireRate.value;
            //animator.Play("Turret Layer."+PewPewAnimation);

            Shoot();
        }
        FireCountDown -= Time.deltaTime;
    }
    public void Shoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
        audioSource.Play();
        for (int i = 0; i < firePoint.Count; i++)
        {
            for (int j = 0; j < target.Count; j++)
            {
                Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
                foreach (var outerKVP in bulletMod)
                {
                    foreach (var innerKVP in bulletMod[outerKVP.Key])
                    {
                        var moddedValue = bulletMod[outerKVP.Key][innerKVP.Key];
                        if (outerKVP.Key == 1)
                        {
                            bullet.bulletDamage.AddModifier(moddedValue);
                            //Debug.Log(bullet.bulletDamage.value);
                        }
                        else if (outerKVP.Key == 2)
                        {
                            bullet.critDamage.AddModifier(moddedValue);
                            //Debug.Log(bullet.bulletDamage.value);
                        }
                        else if (outerKVP.Key == 3)
                        {
                            bullet.critChance.AddModifier(moddedValue);
                        }
                    }
                }
                if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
                {
                    bullet.AddDamageMod(modifier);
                }
                if (bullet != null)
                {
                    bullet.Seek(target[j]);
                }
                //Debug.Log("Final Bullet Damage " + bullet.bulletDamage.value);
            }
        }
    }
    private GameObject MakeBullet(int i)
    {
        //Debug.Log(bu.bulletDamage.value);
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }
    public override void RotateToObject()
    {
        for (int i = 0; i < objectToRotate.Length; i++)
        {
            float angle = Mathf.Atan2(target[0].transform.position.y - objectToRotate[i].transform.position.y, target[0].transform.position.x - objectToRotate[i].transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            objectToRotate[i].transform.rotation = Quaternion.RotateTowards(objectToRotate[i].transform.rotation, targetRotation, rotationSpeed.baseValue * Time.deltaTime);
        }
    }
    #region function for making changes to turret stat
    //Add mod
    public void AddDamageMod(StatModifier mod)//Now add to a dict
    {
        if (!bulletMod[1].ContainsKey(mod.source))//no key yet
        {
            bulletMod[1].Add(mod.source, mod);
        }
        else if (bulletMod[1][mod.source].value < mod.value)
        {
            bulletMod[1][mod.source] = mod;
        }
        //bu.bulletDamage.AddModifier(mod);
        Debug.Log("Damage " + bu.bulletDamage.value);
    }
    public void AddRateMod(StatModifier mod)
    {
        fireRate.AddingOneInstance(mod);
        Debug.Log("Fire rate" + fireRate.value);
    }
    public void AddRangeMod(StatModifier mod)
    {
        range.AddingOneInstance(mod);
        Debug.Log("Range" + range.value);
    }
    public void AddCritDamageMod(StatModifier mod)
    {
        if (!bulletMod[2].ContainsKey(mod.source))
        {
            bulletMod[2].Add(mod.source, mod);
        }
        else if (bulletMod[2][mod.source].value < mod.value)
        {
            bulletMod[2][mod.source] = mod;
        }
        //bu.critDamage.AddModifier(mod);
        Debug.Log("Crit damage" + fireRate.value);
    }
    public void AddCritChanceMod(StatModifier mod)
    {
        if (!bulletMod[3].ContainsKey(mod.source))
        {
            bulletMod[3].Add(mod.source, mod);
        }
        else if (bulletMod[3][mod.source].value < mod.value)
        {
            bulletMod[3][mod.source] = mod;
        }
        //bu.critChance.AddModifier(mod);
        Debug.Log("Crit chance" + fireRate.value);
    }
    //undo
    public void UndoDamageModification(object source)
    {
        bulletMod[1].Remove(source);
        //bu.bulletDamage.RemoveAllModifiersFromSource(source);
    }
    public void UndoRateModification(object source)
    {
        fireRate.RemoveAllModifiersFromSource(source);
        Debug.Log("Remove all " + fireRate.value);
    }
    public void UndoRangeModification(object source)
    {
        range.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritDamageModification(object source)
    {
        bulletMod[2].Remove(source);
        //bu.critDamage.RemoveAllModifiersFromSource(source);
    }
    public void UndoCritChanceModification(object source)
    {
        bulletMod[3].Remove(source);
        //bu.critChance.RemoveAllModifiersFromSource(source);
    }
    //gay method
    public void AddingOneInstanceRateMod(StatModifier mod)
    {
        Debug.Log("Fire rate" + fireRate.value);
        fireRate.AddingOneInstance(mod);

    }
    #endregion

    public Bullet BulletStat()
    {
        return bu;
    }
    public StringBuilder GetDamage()
    {
        StringBuilder text = new StringBuilder();
        text.Append(bu.bulletDamage.value);
        //text.Append("Damage:" + bu.bulletDamage.value + "\n");
        return text;
    }
    public StringBuilder GetROF()
    {
        StringBuilder text = new StringBuilder();
        text.Append(fireRate.value);
        //text.Append("Fire rate:" + fireRate.value + "\n");
        return text;
    }
    public StringBuilder GetRange()
    {
        StringBuilder text = new StringBuilder();
        text.Append(range.value);
        //text.Append("Range:" + range.value + "\n");
        return text;
    }
    public void GetSlow()
    {
        StringBuilder text = new StringBuilder();

        SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
        if (SE.chance < 1)
        {
            text.Append(SE.chance * 100 + "% chance" + " to ");
        }
        if (SE.ID.Contains("SL"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " for " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("STUN"))
        {
            text.Append(SE.description + " for " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("TUR"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + "each shot" + "\n");
        }
        if (SE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + SE.stackTime + "\n");
        }

    }
    public void GetDOTS()
    {
        StringBuilder text = new StringBuilder();

        DotsEffect DE = fxManager.GetDOTSEffect() as DotsEffect;

        if (DE.chance < 1)
        {
            text.Append(DE.chance * 100 + "% chance" + " to ");
        }
        text.Append(DE.description + " " + $"<color=#00ff00ff>{DE.damagePerRate.value}</color>" + " per " + DE.rate.value + "s" + " for " + $"<color=#00ff00ff>{ DE._duration + "s"}</color>" + "\n");
        if (DE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + DE.stackTime + "\n");
            text.Append("Damage increase rate :" + DE.damageIncreaseRate.statValue.value + "\n");
            text.Append("time reduction rate :" + "-" + DE.rateIncrease.statValue.value + "s" + "\n");
        }

    }
    public void GetFear()
    {
        StringBuilder text = new StringBuilder();
        FearEffect FE = fxManager.GetFearEffect() as FearEffect;
        if (FE.chance < 1)
        {
            text.Append(FE.chance * 100 + "% chance" + " to ");
        }
        text.Append(FE.description + " for " + $"<color=#00ff00ff>{ FE._duration + "s"}</color>" + "\n");
        if (FE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + FE.stackTime + "\n");
        }

    }
    public void GetWeaken()
    {
        StringBuilder text = new StringBuilder();

        Weaken WE = fxManager.GetWeakenEffect() as Weaken;
        if (WE.chance < 1)
        {
            text.Append(WE.chance * 100 + "% chance" + " to ");
        }
        text.Append(WE.description);
        if (WE.extraDamageTaken.modType.HasFlag(StatModType.Flat))
        {
            text.Append($"<color=#00ff00ff>{"+" + WE.extraDamageTaken.statValue.value}</color>" + " Damage " + "\n");
        }
        else if (WE.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || WE.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
        {
            text.Append($"<color=#00ff00ff>{"+" + WE.extraDamageTaken.statValue.value + "%"}</color>" + " Damage " + "\n");
        }
        if (WE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + WE.stackTime + "\n");
            text.Append("Extra Damage taken rate :" + " +" + WE.increaseRate.statValue.value + "\n");
            //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
        }
    }
    public void GetArmorBreaking()
    {
        StringBuilder text = new StringBuilder();

        ArmorBreaking ABE = fxManager.GetWeakenEffect() as ArmorBreaking;
        if (ABE.chance < 1)
        {
            text.Append(ABE.chance * 100 + " to ");
        }
        text.Append(ABE.description + " for " + $"<color=#00ff00ff>{ ABE._duration + "s"}</color>" + "\n");

    }
    public StringBuilder GetStatusEffect()
    {
        StringBuilder text = new StringBuilder();

        if (bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking ABE = fxManager.GetWeakenEffect() as ArmorBreaking;
            if (ABE.chance < 1)
            {
                text.Append(ABE.chance * 100 + " to ");
            }
            text.Append(ABE.description + " for " + $"<color=#00ff00ff>{ ABE._duration + "s"}</color>" + "\n");
        }
        return text;
    }
    public Sprite StatusEffectSprite()
    {
        switch (bu.bulletType)
        {
            case BulletType.SlowPerSecond:
                SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
                return SE.Icon;
            case BulletType.Dots:
                DotsEffect DE = fxManager.GetDOTSEffect() as DotsEffect;
                return DE.Icon;
            case BulletType.Fear:
                FearEffect FE = fxManager.GetFearEffect() as FearEffect;
                return FE.Icon;
            case BulletType.Insta_Kill:
                break;
            case BulletType.Weaken:
                Weaken WE = fxManager.GetWeakenEffect() as Weaken;
                return WE.Icon;
            case BulletType.ArmorBreaking:
                ArmorBreaking ABE = fxManager.GetWeakenEffect() as ArmorBreaking;
                return ABE.Icon;
            case BulletType.PiercingShot:
                break;
            default:
                break;
        }
        Debug.LogError("OH FUCK FUCK FUCK NO Sprite");
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), range.Value);
        /* Gizmos.color = Color.green;
         Gizmos.DrawWireSphere(transform.position, Range);*/
    }
#endif
}