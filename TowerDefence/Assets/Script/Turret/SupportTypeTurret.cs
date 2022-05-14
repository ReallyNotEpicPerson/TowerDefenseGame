using System.Collections.Generic;
using UnityEngine;

public class SupportTypeTurret : BaseTurretStat
{
    [SerializeField] private float damageIncreaseMod = 0;
    [SerializeField] private float damageMax = 0.2f;//max += 50%/5
    [SerializeField] private float rateOfFireIncreaseMod = 0;
    [SerializeField] private float rateMax = 0.2f;//max += 30%/5
    [SerializeField] private float rangeIncreaseMod = 0;
    [SerializeField] private float rangeMax = 0.2f;//max += 30%/5
    [SerializeField] private float critDamageIncreaseMod = 0;
    [SerializeField] private float critDamageMax = 0.2f;//max += 60%/5
    [SerializeField] private float critChanceIncreaseMod = 0;
    [SerializeField] private float critChanceMax = 0.2f;//max += 20%/5
    private List<Collider2D> enemyList = new List<Collider2D>();

    private void OnValidate()
    {
        transform.Find("Range").localScale = Vector3.one;
        transform.Find("Range").localScale *= range.value;
        GetComponent<CircleCollider2D>().radius = range.value;
    }
    public override void Awake()
    {
        base.Awake();
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
                        if (damageIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddDamageMod(new StatModifier(damageIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (rateOfFireIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddRateMod(new StatModifier(rateOfFireIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (rangeIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddRangeMod(new StatModifier(rangeIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (critDamageIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddCritDamageMod(new StatModifier(critDamageIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (critChanceIncreaseMod != 0)
                        {
                            lazerTypeTurret.AddCritChanceMod(new StatModifier(critChanceIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        break;
                    case BulletTypeTurret bulletTypeTurret:
                        if (damageIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddDamageMod(new StatModifier(damageIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (rateOfFireIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddRateMod(new StatModifier(rateOfFireIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (rangeIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddRangeMod(new StatModifier(rangeIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (critDamageIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddCritDamageMod(new StatModifier(critDamageIncreaseMod, StatModType.PercentBuffBest, this));
                        }
                        if (critChanceIncreaseMod != 0)
                        {
                            bulletTypeTurret.AddCritChanceMod(new StatModifier(critChanceIncreaseMod, StatModType.PercentBuffBest, this));
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
    public override void UpdateTarget()
    {
        target.Clear();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, range.value);
        foreach (Collider2D col in collider)
        {
            if (col.TryGetComponent(out Enemy enemy) && enemy.enemyType.HasFlag(EnemyType.Invisible))
            {
                if (!enemyList.Contains(col))
                {
                    enemyList.Add(col);
                }
                if (enemy.enemyState.HasFlag(EnemyState.Invisible))
                {
                    enemy.DeInvisible();
                }
            }
        }
    }
    public void upgrade(int i)
    {
        switch (i)
        {
            case 0:
                damageIncreaseMod += damageMax / 5;
                break;
            case 1:
                rateOfFireIncreaseMod += rateMax / 5;
                break;
            case 2:
                rangeIncreaseMod += rangeMax / 5;
                break;
            case 3:
                critDamageIncreaseMod += critDamageMax / 5;
                break;
            case 4:
                critChanceIncreaseMod += critChanceMax / 5;
                break;
            default:
                break;
        }
        UpdateTurret();
    }
    public void MassUndo()
    {
        for (int i = 0; i < target.Count; i++)
        {
            BaseTurretStat turret = target[i].GetComponent<BaseTurretStat>();
            switch (turret)
            {
                case LazerTypeTurret lazerTypeTurret:
                    //lazerTypeTurret.spriteRenderer.color = Color.red;
                    lazerTypeTurret.UndoDamageModification(this);
                    lazerTypeTurret.UndoRateModification(this);
                    lazerTypeTurret.UndoRangeModification(this);
                    lazerTypeTurret.UndoCritDamageModification(this);
                    lazerTypeTurret.UndoCritChanceModification(this);
                    break;
                case BulletTypeTurret bulletTypeTurret:
                    //bulletTypeTurret.spriteRenderer.color = Color.red;
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
    public override void OnMouseEnter()
    {
        return;
    }
    public override void OnMouseExit()
    {
        return;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You`re in range");
        /*if (collision.TryGetComponent(out Enemy ene))
        {
            if (!ene.enemyType.HasFlag(EnemyType.Invisible))
            {
                return;
            }
            Debug.Log("Gotcha fucker!");
            //ene.DeInvisible();
        }*/
        //else 
        if (collision.TryGetComponent(out BaseTurretStat _))
        {
            Debug.Log("A MTF turret " + collision.transform.position);
            UpdateTurret();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("You`re not in range");
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (Vector3.SqrMagnitude(transform.position - enemyList[i].transform.position) > range.value)
            {
                enemyList[i].GetComponent<Enemy>().Invisible();
                enemyList.RemoveAt(i);
            }
        }
    }
}

