using UnityEditor;
using UnityEngine;

public class SupportTypeTurret : BaseTurretStat
{
    public StatModType damageType;
    [SerializeField] private float damageIncreaseMod=0;
    public StatModType rofType;
    [SerializeField] private float rateOfFireIncreaseMod=0;
    public StatModType rangeType;
    [SerializeField] private float rangeIncreaseMod=0;
    public StatModType critDamageType;
    [SerializeField] private float critDamageIncreaseMod=0;
    public StatModType critChanceType;
    [SerializeField] private float critChanceIncreaseMod=0;

    private void OnValidate()
    {
        transform.Find("Range").localScale = Vector3.one;
        transform.Find("Range").localScale *= range.value;
        GetComponent<CircleCollider2D>().radius = range.value;
    }

    public override void Awake()
    {
        base.Awake();
        UpdateTurret();
    }
    public void UpdateTurret()
    {
        target.Clear();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out BaseTurretStat turret))
            {
                switch (turret)
                {
                    case LazerTypeTurret lazerTypeTurret:
                        if (!lazerTypeTurret.CheckDamageModsource(this) && damageIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddDamageMod(new StatModifier(damageIncreaseMod, damageType, this));
                        }
                        if (!lazerTypeTurret.CheckRateModsource(this) && rateOfFireIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddRateMod(new StatModifier(rateOfFireIncreaseMod, rofType, this));
                        }
                        if (!lazerTypeTurret.CheckRangeModsource(this) && rangeIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddRangeMod(new StatModifier(rangeIncreaseMod, rangeType, this));
                        }
                        if (!lazerTypeTurret.CheckCritDamageModsource(this) && critDamageIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddCritDamageMod(new StatModifier(critDamageIncreaseMod, critDamageType, this));
                        }
                        if (!lazerTypeTurret.CheckCritChanceModsource(this) && critChanceIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddCritChanceMod(new StatModifier(critChanceIncreaseMod, critChanceType, this));
                        }
                        break;
                    case BulletTypeTurret bulletTypeTurret:
                        if (!bulletTypeTurret.CheckDamageModsource(this) && damageIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddDamageMod(new StatModifier(damageIncreaseMod, damageType, this));
                        }
                        if (!bulletTypeTurret.CheckRateModsource(this) && rateOfFireIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddRateMod(new StatModifier(rateOfFireIncreaseMod, rofType, this));
                        }
                        if (!bulletTypeTurret.CheckRangeModsource(this) && rangeIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddRangeMod(new StatModifier(rangeIncreaseMod, rangeType, this));
                        }
                        if (!bulletTypeTurret.CheckCritDamageModsource(this) && critDamageIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddCritDamageMod(new StatModifier(critDamageIncreaseMod, critDamageType, this));
                        }
                        if (!bulletTypeTurret.CheckCritChanceModsource(this) && critChanceIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddCritChanceMod(new StatModifier(critChanceIncreaseMod, critChanceType, this));
                        }
                        break;
                    default:
                        break;
                }
                target.Add(col.transform);
            }
            else
            {
                continue;
            }
        }
        Debug.Log("Turret in range :" + target.Count);
    }

    public void MassUndo()
    {
        for (int i = 0; i < target.Count; i++)
        {
            BaseTurretStat turret = target[i].GetComponent<BaseTurretStat>();
            switch (turret)
            {
                case LazerTypeTurret lazerTypeTurret:
                    lazerTypeTurret.UndoDamageModification(this);
                    lazerTypeTurret.UndoRateModification(this);
                    lazerTypeTurret.UndoRangeModification(this);
                    lazerTypeTurret.UndoCritDamageModification(this);
                    lazerTypeTurret.UndoCritChanceModification(this);
                    break;
                case BulletTypeTurret bulletTypeTurret:
                    bulletTypeTurret.UndoDamageModification(this);
                    bulletTypeTurret.UndoRateModification(this);
                    bulletTypeTurret.UndoRangeModification(this);
                    bulletTypeTurret.UndoCritDamageModification(this);
                    bulletTypeTurret.UndoCritChanceModification(this);
                    break;
                default:
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You`re in range");
        if(collision.TryGetComponent(out Enemy ene))
        {
            if (!ene.enemyType.HasFlag(EnemyType.Invisible))
            {
                return;
            }
            Debug.Log("Gotcha fucker!");
            ene.DeInvisible();
        }
        else if (collision.TryGetComponent(out BaseTurretStat _))
        {
            Debug.Log("A MTF turret");
            UpdateTurret();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("You`re not in range");
        if (collision.TryGetComponent(out Enemy ene))
        {
            if (!ene.enemyType.HasFlag(EnemyType.Invisible))
            {
                return;
            }
            if (ene.IsInvisible())
            {
                ene.Invisible();
            }
        }        
    }
}

