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
        FireCountDown = 1 / fireRate.value;
    }
    void Update()
    {
        if (  target.Count == 0 || target[0]==null)
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
    public StringBuilder GetStatusEffect()
    {
        StringBuilder text = new StringBuilder();
        if (bu.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect SE = fxManager.GetSlowEffect() as SlowEffect;
            /*
            if (SE._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
            }
            if (SE._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
            }

            if (SE._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
            {
                if (SE.ID.Contains("SL"))
                {
                    text.Append("Slow:");
                }
                else if (SE.ID.Contains("TUR"))
                {
                    text.Append("SpeedBoost:");
                }
                text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{SE._duration + "s"}</color>" + "\n");
            }*/
            if (SE.chance < 1)
            {
                text.Append(SE.chance * 100 + "%" + " to ");
            }
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ SE._duration + "s"}</color>" + "\n");
            if (SE.effectType.HasFlag(EffectType.StackingEffect))
            {
                text.Append("Can stack to " + SE.stackTime + "\n");
            }
        }
        /*if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up = upgradeVersion.fxManager.GetDOTSEffect() as DotsEffect;
            if (bu.bulletType.HasFlag(BulletType.Dots))
            {
                DotsEffect pre = this.fxManager.GetDOTSEffect() as DotsEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.damagePerRate.value > pre.damagePerRate.value)//Better
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#00ff00ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.damagePerRate.value < pre.damagePerRate.value)
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#ff0000ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)//Better
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#00ff00ff>{up.rate.value + "s" }</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#ff0000ff>{up.rate.value + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up.damagePerRate.value }</color>" + " damage per " + $"<color=#00ff00ff>{ up.rate.value + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Fear))
        {
            FearEffect up = upgradeVersion.fxManager.GetFearEffect() as FearEffect;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                FearEffect pre = fxManager.GetFearEffect() as FearEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    //text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Weaken))
        {
            Weaken up = upgradeVersion.fxManager.GetWeakenEffect() as Weaken;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                Weaken pre = fxManager.GetWeakenEffect() as Weaken;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description);
                //+ $"<color=#00ff00ff>{up.extraDamageTaken.statValue}</color>");
                if (up.extraDamageTaken.modType.HasFlag(StatModType.Flat))
                {
                    text.Append($"<color=#00ff00ff>{"+" + up.extraDamageTaken.statValue.value}</color>" + "\n");
                }
                else if (up.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || up.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
                {
                    text.Append($"<color=#00ff00ff>{up.extraDamageTaken.statValue.value + "%"}</color>" + "\n");
                }
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Extra Damage taken rate :" + " +" + up.increaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking up = upgradeVersion.fxManager.GetWeakenEffect() as ArmorBreaking;
            if (bu.bulletType.HasFlag(BulletType.ArmorBreaking))
            {
                ArmorBreaking pre = fxManager.GetArmorBreakEffect() as ArmorBreaking;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
            }
        }*/
        return text;
    }
    public StringBuilder CompareTurretStat(BulletTypeTurret upgradeVersion)
    {
        StringBuilder text = new StringBuilder();
        if (upgradeVersion.bu.bulletDamage.baseValue > this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage:" + bu.bulletDamage.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletDamage.baseValue < this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage:" + bu.bulletDamage.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue > this.bu.critChance.baseValue)
        {
            text.Append("Crit%:" + bu.critChance.baseValue + "%" + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue < this.bu.critChance.baseValue)
        {
            text.Append("Crit%:" + bu.critChance.baseValue + "%" + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue > fireRate.baseValue)
        {
            text.Append("Fire rate:" + fireRate.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue < fireRate.baseValue)
        {
            text.Append("Fire rate:" + fireRate.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue > range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue < range.baseValue)
        {
            text.Append("Range:" + range.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        //StatusEffect 
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Insta_Kill))
        {
            Insta_Kill up = upgradeVersion.fxManager.GetInsta_KillEffect() as Insta_Kill;
            if (bu.bulletType.HasFlag(BulletType.Insta_Kill))
            {
                Insta_Kill pre = this.fxManager.GetInsta_KillEffect() as Insta_Kill;
                if (up.chance > pre.chance)
                {
                    text.Append("Insta-kill Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%"}</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Insta-kill Chance:" + pre.chance * 100 + " %" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                text.Append(up.description + " " + $"<color=#2596beff>{up.chance * 100 + " %"}</color>" + "\n");
            }
            //+ $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up = upgradeVersion.fxManager.GetSlowEffect() as SlowEffect;
            if (bu.bulletType.HasFlag(BulletType.SlowPerSecond))
            {
                SlowEffect pre = this.fxManager.GetSlowEffect() as SlowEffect;
                if (up.chance > pre.chance)//better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
                }
                if (up._duration > pre._duration)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s" }</color>" + "\n");
                }
                if (up._duration < pre._duration)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Duration:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("Duration:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value > pre._slowPercentage.statValue.value)//better
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#00ff00ff>{up._duration + "s"}</color>" + "\n");
                }
                if (up._slowPercentage.statValue.value < pre._slowPercentage.statValue.value)
                {
                    if (up.ID.Contains("SL"))
                    {
                        text.Append("Slow:");
                    }
                    else if (up.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost:");
                    }
                    text.Append(pre._duration + "s" + "->" + $"<color=#ff0000ff>{up._duration + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + "%" + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ up._duration + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up = upgradeVersion.fxManager.GetDOTSEffect() as DotsEffect;
            if (bu.bulletType.HasFlag(BulletType.Dots))
            {
                DotsEffect pre = this.fxManager.GetDOTSEffect() as DotsEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.damagePerRate.value > pre.damagePerRate.value)//Better
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#00ff00ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.damagePerRate.value < pre.damagePerRate.value)
                {
                    if (up.ID.Contains("POI"))
                    {
                        text.Append("Poison damage:");
                    }
                    else if (up.ID.Contains("BU"))
                    {
                        text.Append("Burn damage:");
                    }
                    text.Append(pre.damagePerRate.value + "->" + $"<color=#ff0000ff>{up.damagePerRate.value}</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)//Better
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#00ff00ff>{up.rate.value + "s" }</color>" + "\n");
                }
                if (up.rate.value > pre.rate.value)
                {
                    text.Append("rate:" + pre.rate.value + "s" + "->" + $"<color=#ff0000ff>{up.rate.value + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + " " + $"<color=#00ff00ff>{up.damagePerRate.value }</color>" + " damage per " + $"<color=#00ff00ff>{ up.rate.value + "s"}</color>" + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Fear))
        {
            FearEffect up = upgradeVersion.fxManager.GetFearEffect() as FearEffect;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                FearEffect pre = fxManager.GetFearEffect() as FearEffect;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    //text.Append("Damage increase rate :" + up.damageIncreaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Weaken))
        {
            Weaken up = upgradeVersion.fxManager.GetWeakenEffect() as Weaken;
            if (bu.bulletType.HasFlag(BulletType.Fear))
            {
                Weaken pre = fxManager.GetWeakenEffect() as Weaken;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description);
                //+ $"<color=#00ff00ff>{up.extraDamageTaken.statValue}</color>");
                if (up.extraDamageTaken.modType.HasFlag(StatModType.Flat))
                {
                    text.Append($"<color=#00ff00ff>{"+" + up.extraDamageTaken.statValue.value}</color>" + "\n");
                }
                else if (up.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || up.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
                {
                    text.Append($"<color=#00ff00ff>{up.extraDamageTaken.statValue.value + "%"}</color>" + "\n");
                }
                if (up.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up.stackTime + "\n");
                    text.Append("Extra Damage taken rate :" + " +" + up.increaseRate.statValue.value + "\n");
                    //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {
            ArmorBreaking up = upgradeVersion.fxManager.GetWeakenEffect() as ArmorBreaking;
            if (bu.bulletType.HasFlag(BulletType.ArmorBreaking))
            {
                ArmorBreaking pre = fxManager.GetArmorBreakEffect() as ArmorBreaking;
                if (up.chance > pre.chance)//Better
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
                if (up.chance < pre.chance)
                {
                    text.Append("Chance:" + pre.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                if (up.chance < 1)
                {
                    text.Append(up.chance * 100 + " to ");
                }
                text.Append(up.description + "\n");
            }
        }
        return text;
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