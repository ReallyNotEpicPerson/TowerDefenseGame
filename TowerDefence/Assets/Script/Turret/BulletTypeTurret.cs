#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;
//rangeTimer
public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public CharacterStat fireRate;
    [Header("List of Fire Point")]
    public List<Transform> firePoint;
    [Header("Gameobject stuff")]
    public GameObject[] objectToRotate;
    [SerializeField] private Animator animator;
    public GameObject BulletPrefab;
    [SerializeField] private Bullet bu;
    public float chanceToQuadrupleDamage = 0;

    private EffectManager fxManager;
    private float FireCountDown = 0f;
    private EntityEffectHandler fxHandler;
    private StatModifier modifier;
    private Dictionary<int, Dictionary<object, StatModifier>> bulletMod =
        new Dictionary<int, Dictionary<object, StatModifier>>();
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

    }//range
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
            if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamageEachShot))
            {
                fxHandler.AddDebuff(fxManager.GetDamageBoostData(), gameObject);
            }
            FireCountDown = 1 / fireRate.value;
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
                if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamageEachShot))
                {
                    bullet.AddDamageMod(modifier);
                }
                if (passiveAbility.HasFlag(PassiveAbility.QuadrupleDamage) && Random.value <= chanceToQuadrupleDamage)
                {
                    bullet.quadrupleDamage = true;
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
        //Debug.Log("Fire rate" + fireRate.value);
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
    public StringBuilder GetCritChance()
    {
        StringBuilder text = new StringBuilder();
        text.Append(bu.critChance.value * 100 + "%");
        return text;
    }
    public StringBuilder GetCritDamage()
    {
        StringBuilder text = new StringBuilder();
        text.Append("+" + bu.critDamage.value * 100 + "%");
        return text;
    }
    public StringBuilder GetBulletSpeed()
    {
        StringBuilder text = new StringBuilder();
        text.Append(bu.bulletSpeed.value);
        return text;
    }
    public StringBuilder GetSlow()
    {
        StringBuilder text = new StringBuilder();
        SlowEffect SE = bu.fxManager.GetSlowEffect() as SlowEffect;
        if (SE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{SE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        if (SE.ID.Contains("SL"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.value * 100 + "%"}</color>" + " for " + $"<color=#00ff00ff>{ SE.duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("STUN"))
        {
            text.Append(SE.description + " for " + $"<color=#00ff00ff>{ SE.duration + "s"}</color>" + "\n");
        }
        else if (SE.ID.Contains("TUR"))
        {
            text.Append(SE.description + " " + $"<color=#00ff00ff>{SE._slowPercentage.statValue.baseValue * 100 + "%"}</color>" + " each shot" + "\n");
        }
        if (SE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + SE.stackTime + "\n");
        }
        return text;
    }
    public StringBuilder GetDOTS()
    {
        StringBuilder text = new StringBuilder();
        DotsEffect DE = bu.fxManager.GetDOTSEffect() as DotsEffect;
        if (DE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{DE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        text.Append(DE.description + " " + $"<color=#00ff00ff>{DE.damagePerRate.value / DE.rate.value}</color>" + " damage for " + $"<color=#00ff00ff>{ DE.duration + "s"}</color>" + "\n");
        if (DE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{DE.stackTime}</color>" + "\n");
            text.Append("Damage increase rate :" + $"<color=#00ff00ff>{DE.damageIncreaseRate.statValue.value}</color>" + "\n");
            text.Append("time reduction rate :" + "-" + $"<color=#00ff00ff>{DE.rateIncrease.statValue.value}</color>" + "s" + "\n");
        }
        return text;
    }
    public StringBuilder GetFear()
    {
        StringBuilder text = new StringBuilder();
        FearEffect FE = bu.fxManager.GetFearEffect() as FearEffect;
        if (FE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{FE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        text.Append(FE.description + " for " + $"<color=#00ff00ff>{ FE.duration + "s"}</color>" + "\n");
        if (FE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{FE.stackTime}</color>" + "\n");
        }
        return text;
    }
    public StringBuilder GetInstaKill()
    {
        StringBuilder text = new StringBuilder();
        Insta_Kill IK = bu.fxManager.GetInsta_KillEffect() as Insta_Kill;
        if (IK.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{IK.chance * 100 + "%"}</color>" + " chance" + " to " + IK.description);
        }
        return text;
    }
    public StringBuilder GetWeaken()
    {
        StringBuilder text = new StringBuilder();

        Weaken WE = bu.fxManager.GetWeakenEffect() as Weaken;
        if (WE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{WE.chance * 100 + "%"}</color>" + " chance" + " to ");
        }
        text.Append(WE.description);
        if (WE.extraDamageTaken.modType.HasFlag(StatModType.Flat))
        {
            text.Append($"<color=#00ff00ff>{" +" + WE.extraDamageTaken.statValue.value}</color>" + " Damage " + "\n");
        }
        else if (WE.extraDamageTaken.modType.HasFlag(StatModType.PercentAdd) || WE.extraDamageTaken.modType.HasFlag(StatModType.PercentMult))
        {
            text.Append($"<color=#00ff00ff>{" +" + WE.extraDamageTaken.statValue.value * 100 + "%"}</color>" + " Damage " + "\n");
        }
        if (WE.effectType.HasFlag(EffectType.StackingEffect))
        {
            text.Append("Can stack to " + $"<color=#00ff00ff>{WE.stackTime}</color>" + "\n");
            text.Append("Extra Damage taken rate :" + " +" + WE.increaseRate.statValue.value + "\n");
            //text.Append("time reduction rate :" + "-" + up.rateIncrease.statValue.value + "s" + "\n");
        }
        return text;
    }
    public StringBuilder GetArmorBreaking()
    {
        StringBuilder text = new StringBuilder();
        ArmorBreaking ABE = bu.fxManager.GetArmorBreakEffect() as ArmorBreaking;
        if (ABE.chance < 1)
        {
            text.Append($"<color=#00ff00ff>{ABE.chance * 100 + "%"}</color>" + " chance to ");
        }
        text.Append(ABE.description + " for " + $"<color=#00ff00ff>{ ABE.duration + "s"}</color>" + "\n");
        if (ABE.effectType.HasFlag(EffectType.StackableDuration))
        {
            text.Append("Can Stack up duration" + "\n");
        }
        return text;
    }
    public StringBuilder GetStatusEffect()
    {//Slow+DOTS+Fear+Weaken+.ArmorBreaking+Insta_Kill+Piercingshot+
        StringBuilder text = new StringBuilder();
        if (bu.bulletType.HasFlag(StatusEffectType.SlowPerSecond))
        {
            text.Append(GetSlow());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.Dots))
        {
            text.Append(GetDOTS());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.Fear))
        {
            text.Append(GetFear());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.Weaken))
        {
            text.Append(GetWeaken());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.Insta_Kill))
        {
            text.Append(GetInstaKill());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.ArmorBreaking))
        {
            text.Append(GetArmorBreaking());
        }
        if (bu.bulletType.HasFlag(StatusEffectType.PiercingShot))
        {
            text.Append("Ignore ememy armor" + "\n");
        }
        if (bu.bulletType.HasFlag(StatusEffectType.ArmorDestroyer))
        {
            text.Append("Destroy enemy armor " + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.CanShootWhenBuy))
        {
            text.Append("Cant be place on the field,but can shoot when bought" + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.CanSeeInvisibleUnit))
        {
            text.Append("Can see invisible enemy" + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.QuadrupleDamage))
        {
            text.Append("Have " + $"<color=#00ff00ff>{chanceToQuadrupleDamage * 100 + "%" }</color>" + " chance to quadruple damage" + "\n");
        }
        if (passiveAbility.HasFlag(PassiveAbility.InfiniteRange))
        {
            text.Append("Turret have infinite range" + "\n");
        }
        if (text.Length == 0)
        {
            text.Append("None" + "\n");
        }
        return text;
    }
    public Sprite StatusEffectSprite()
    {
        switch (bu.bulletType)
        {
            case StatusEffectType.SlowPerSecond:
                SlowEffect SE = bu.fxManager.GetSlowEffect() as SlowEffect;
                return SE.Icon;
            case StatusEffectType.Dots:
                DotsEffect DE = bu.fxManager.GetDOTSEffect() as DotsEffect;
                return DE.Icon;
            case StatusEffectType.Fear:
                FearEffect FE = bu.fxManager.GetFearEffect() as FearEffect;
                return FE.Icon;
            case StatusEffectType.Insta_Kill:
                Insta_Kill IK = bu.fxManager.GetInsta_KillEffect() as Insta_Kill;
                return IK.Icon;
            case StatusEffectType.Weaken:
                Weaken WE = bu.fxManager.GetWeakenEffect() as Weaken;
                return WE.Icon;
            case StatusEffectType.ArmorBreaking:
                ArmorBreaking ABE = bu.fxManager.GetArmorBreakEffect() as ArmorBreaking;
                return ABE.Icon;
            case StatusEffectType.PiercingShot:
                break;
            default:
                break;
        }
        Debug.LogError("OH FUCK FUCK FUCK NO Sprite");
        return null;
    }
    public StringBuilder GetTargetingType()
    {
        StringBuilder text = new StringBuilder();
        text.Append("Targeting Type:" + "\n");
        if (targetingType.HasFlag(TargetingType.Armored))
        {
            text.Append("Prioritised Amored Enemy" + "\n");
        }
        if (targetingType.HasFlag(TargetingType.Closest))
        {
            text.Append("Target the closest Enemy" + "\n");
        }
        if (targetingType.HasFlag(TargetingType.First))
        {
            text.Append("Target the first Enemy" + "\n");
        }
        if (targetingType.HasFlag(TargetingType.LeastHealth))
        {
            text.Append("Target the weakest Enemy" + "\n");
        }
        if (targetingType.HasFlag(TargetingType.MostHealth))
        {
            text.Append("Target the strongest Enemy" + "\n");
        }
        if (targetingType.HasFlag(TargetingType.Random))
        {
            text.Append("Target random Enemy" + "\n");
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