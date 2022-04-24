#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class BulletTypeTurret : BaseTurretStat
{
    [Header("BulletTypeStat")]
    public List<Transform> firePoint;//keep at all cost
    public CharacterStat fireRate;
    public GameObject BulletPrefab;
    [SerializeField] private Bullet bu;
    private float FireCountDown = 0f;
    private EntityEffectHandler fxHandler;
    private EffectManager fxManager;
    private StatModifier modifier;

    private Animator animator;
    //public string PewPewAnimation;
    //private ObjectPooler pool;
    public void OnValidate()
    {
        BulletPrefab.TryGetComponent(out bu);
        TryGetComponent(out fxManager);
        TryGetComponent(out fxHandler);
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
        transform.Find("Range").localScale *= range.baseValue;
        //transform.GetChild(1).localScale=Vector3.one;
        //transform.GetChild(1).localScale*=range.baseValue;
    }
    public override void Awake()
    {
        base.Awake();
        TryGetComponent(out fxHandler);
        TryGetComponent(out fxManager);
        //BulletPrefab.TryGetComponent(out bu);
        //TryGetComponent(out animator);
    }

    void Start()
    {
        //InvokeRepeating("UpdateTarget", 0f, 0.2f);
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }
    void UpdateTarget()
    {
        target.Clear();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        float shortestDis = Mathf.Infinity;
        Collider2D nearestCol = null;
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent<Enemy>(out _))
            {
                if (shootType.HasFlag(ShootType.SingleTarget))
                {
                    float DisToenenmy = Vector3.Distance(transform.position, col.transform.position);//use Distancesquared??
                    if (DisToenenmy < shortestDis)
                    {
                        shortestDis = DisToenenmy;
                        nearestCol = col;
                    }
                }
                else if (shootType.HasFlag(ShootType.MultipleTarget))
                {
                    target.Add(col.transform);
                    if (target.Count == numberOfTarget)
                    {
                        return;
                    }
                }
            }
        }
        if (shootType.HasFlag(ShootType.SingleTarget) && nearestCol != null)
        {
            target.Add(nearestCol.transform);
        }
    }
    void Update()
    {
        if (target.Count == 0)
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
        //animator.SetTrigger("Shoot");
        for (int i = 0; i < firePoint.Count; i++)
        {
            for (int j = 0; j < target.Count; j++)
            {
                Bullet bullet = MakeBullet(i).GetComponent<Bullet>();
                if (passiveAbility.HasFlag(PassiveAbility.IncreaseDamage))
                {
                    bullet.AddDamageMod(modifier);
                }
                if (bullet != null)
                {
                    bullet.Seek(target[j]);
                }
            }
        }
    }
    private GameObject MakeBullet(int i)
    {
        return Instantiate(BulletPrefab, firePoint[i].position, firePoint[i].rotation);
        //pool.SpawnFromPool(BulletPrefab.name, firePoint[i].position, firePoint[i].rotation);
    }
    public void ReduceRate(StatModifier mod)
    {
        fireRate.AddingOneInstance(mod);
        Debug.Log("Just added " + fireRate.value);
    }
    public void UndoModificationToFireRate(object source)
    {
        fireRate.RemoveAllModifiersFromSource(source);
        Debug.Log("Remove all " + fireRate.value);
    }
    public Bullet BulletStat()
    {
        return bu;
    }
    public StringBuilder CompareTurretStat(BulletTypeTurret upgradeVersion)
    {
        StringBuilder text = new StringBuilder();
        if (upgradeVersion.bu.bulletDamage.baseValue > this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage: " + bu.bulletDamage.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletDamage.baseValue < this.bu.bulletDamage.baseValue)
        {
            text.Append("Damage: " + bu.bulletDamage.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.bulletDamage.baseValue }</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue > this.bu.critChance.baseValue)
        {
            text.Append("Crit%: " + bu.critChance.baseValue + "%" + "->" + $"<color=#00ff00ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.bu.critChance.baseValue < this.bu.critChance.baseValue)
        {
            text.Append("Crit%: " + bu.critChance.baseValue + "%" + "->" + $"<color=#ff0000ff>{upgradeVersion.bu.critChance.baseValue + "%"}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue > fireRate.baseValue)
        {
            text.Append("Fire rate: " + fireRate.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.fireRate.baseValue < fireRate.baseValue)
        {
            text.Append("Fire rate: " + fireRate.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.fireRate.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue > range.baseValue)
        {
            text.Append("Range: " + range.baseValue + "->" + $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.range.baseValue < range.baseValue)
        {
            text.Append("Range: " + range.baseValue + "->" + $"<color=#ff0000ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        //StatusEffect 
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Insta_Kill))
        {
            Insta_Kill up_IK = upgradeVersion.fxManager.GetInsta_KillEffect() as Insta_Kill;
            if (bu.bulletType.HasFlag(BulletType.Insta_Kill))
            {
                Insta_Kill pre_IK = this.fxManager.GetInsta_KillEffect() as Insta_Kill;
                if (up_IK.chance > pre_IK.chance)
                {
                    text.Append("Insta-kill Chance: " + pre_IK.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up_IK.chance * 100 + "%"}</color>" + "\n");
                }
                if (up_IK.chance < pre_IK.chance)
                {
                    text.Append("Insta-kill Chance " + pre_IK.chance * 100 + " %" + "->" + $"<color=#ff0000ff>{up_IK.chance * 100 + "%" }</color>" + "\n");
                }
            }
            else
            {
                text.Append(up_IK.description + " " + $"<color=#2596beff>{up_IK.chance * 100 + " %"}</color>" + "\n");
            }
            //+ $"<color=#00ff00ff>{upgradeVersion.range.baseValue}</color>" + "\n");
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.SlowPerSecond))
        {
            SlowEffect up_SL = (SlowEffect)upgradeVersion.fxManager.GetSlowEffect();
            if (bu.bulletType.HasFlag(BulletType.SlowPerSecond))
            {
                SlowEffect pre_SL = this.fxManager.GetSlowEffect() as SlowEffect;
                if (up_SL.chance > pre_SL.chance)//better
                {
                    text.Append("Chance: " + pre_SL.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up_SL.chance * 100 + "%" }</color>" + "\n");
                }
                if (up_SL.chance < pre_SL.chance)
                {
                    text.Append("Chance: " + pre_SL.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up_SL.chance * 100 + "%" }</color>" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value > pre_SL._slowPercentage.statValue.value)//better
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#00ff00ff>{up_SL._slowPercentage.statValue.value * 100 + "%" }</color>" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value < pre_SL._slowPercentage.statValue.value)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._slowPercentage.statValue.value * 100 + "%" + "->" + $"<color=#ff0000ff>{up_SL._slowPercentage.statValue.value * 100 + "%"}</color>" + "\n");
                }
                if (up_SL._duration > pre_SL._duration)//better
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#00ff00ff>{up_SL._duration + "s" }</color>" + "\n");
                }
                if (up_SL._duration < pre_SL._duration)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#ff0000ff>{up_SL._duration + "s"}</color>" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value > pre_SL._slowPercentage.statValue.value)//better
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#00ff00ff>{up_SL._duration + "s"}</color>" + "\n");
                }
                if (up_SL._slowPercentage.statValue.value < pre_SL._slowPercentage.statValue.value)
                {
                    if (up_SL.ID.Contains("SL"))
                    {
                        text.Append("Slow: ");
                    }
                    else if (up_SL.ID.Contains("TUR"))
                    {
                        text.Append("SpeedBoost: ");
                    }
                    text.Append(pre_SL._duration + "s" + "->" + $"<color=#ff0000ff>{up_SL._duration + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up_SL.chance < 1)
                {
                    text.Append(up_SL.chance * 100 + "%" + " to ");
                }
                text.Append(up_SL.description + " " + $"<color=#00ff00ff>{up_SL._slowPercentage.statValue.value * 100 + "%"}</color>" + " in " + $"<color=#00ff00ff>{ up_SL._duration + "s"}</color>" + "\n");
                if (up_SL.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up_SL.stackTime + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Dots))
        {
            DotsEffect up_DOTS = (DotsEffect)upgradeVersion.fxManager.GetDOTSEffect();
            if (bu.bulletType.HasFlag(BulletType.Dots))
            {
                DotsEffect pre_DOTS = this.fxManager.GetDOTSEffect() as DotsEffect;
                if (up_DOTS.chance > pre_DOTS.chance)//Better
                {
                    text.Append("Chance: " + pre_DOTS.chance * 100 + "%" + "->" + $"<color=#00ff00ff>{up_DOTS.chance * 100 + "%" }</color>" + "\n");
                }
                if (up_DOTS.chance < pre_DOTS.chance)
                {
                    text.Append("Chance: " + pre_DOTS.chance * 100 + "%" + "->" + $"<color=#ff0000ff>{up_DOTS.chance * 100 + "%" }</color>" + "\n");
                }
                if (up_DOTS.damagePerRate.value > pre_DOTS.damagePerRate.value)//Better
                {
                    if (up_DOTS.ID.Contains("POI"))
                    {
                        text.Append("Poison damage: ");
                    }
                    else if (up_DOTS.ID.Contains("BU"))
                    {
                        text.Append("Burn damage: ");
                    }
                    text.Append(pre_DOTS.damagePerRate.value + "->" + $"<color=#00ff00ff>{up_DOTS.damagePerRate.value}</color>" + "\n");
                }
                if (up_DOTS.damagePerRate.value < pre_DOTS.damagePerRate.value)
                {
                    if (up_DOTS.ID.Contains("POI"))
                    {
                        text.Append("Poison damage: ");
                    }
                    else if (up_DOTS.ID.Contains("BU"))
                    {
                        text.Append("Burn damage: ");
                    }
                    text.Append(pre_DOTS.damagePerRate.value + "->" + $"<color=#ff0000ff>{up_DOTS.damagePerRate.value}</color>" + "\n");
                }
                if (up_DOTS.rate.value > pre_DOTS.rate.value)//Better
                {
                    text.Append("rate: " + pre_DOTS.rate.value + "s" + "->" + $"<color=#00ff00ff>{up_DOTS.rate.value + "s" }</color>" + "\n");
                }
                if (up_DOTS.rate.value > pre_DOTS.rate.value)
                {
                    text.Append("rate: " + pre_DOTS.rate.value + "s" + "->" + $"<color=#ff0000ff>{up_DOTS.rate.value + "s"}</color>" + "\n");
                }
            }
            else
            {
                if (up_DOTS.chance < 1)
                {
                    text.Append(up_DOTS.chance * 100 + " to ");
                }

                text.Append(up_DOTS.description + " " + $"<color=#00ff00ff>{up_DOTS.damagePerRate.value }</color>" + " damage per " + $"<color=#00ff00ff>{ up_DOTS.rate.value + "s"}</color>" + "\n");
                if (up_DOTS.effectType.HasFlag(EffectType.StackingEffect))
                {
                    text.Append("Can stack to " + up_DOTS.stackTime + "\n");
                    text.Append("Damage increase rate :" + up_DOTS.damageIncreaseRate.statValue.value + "\n");
                    text.Append("time reduction rate :" + "-" + up_DOTS.rateIncrease.statValue.value + "s" + "\n");
                }
            }
        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Fear))
        {

        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.Weaken))
        {

        }
        if (upgradeVersion.bu.bulletType.HasFlag(BulletType.ArmorBreaking))
        {

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