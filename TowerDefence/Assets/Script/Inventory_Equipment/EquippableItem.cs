using UnityEngine;
public enum EquipmentType
{
    Gun_1,
    Gun_2,
    Knife_1,
    Knife_2,
    Accessory_1,
    Material_1,
    material_2,
}
[CreateAssetMenu]
public class EquippableItem : Items
{
    public int StrengthBonus;
    public int BullshitBonus;
    public int IDKBonus;
    public int Like4realBonus;

    [Space]

    public float StrengthBonusPercentage;

    [Space]

    public float BonusPercentage_1;
    public float BonusPercentage_2;
    public float BonusPercentage_3;

    [Space]

    public EquipmentType equipmentType;

    public override Items GetCopy()
    {
        return Instantiate(this);
    }
    public override void Destroy()
    {
        Destroy(this);
    }
    public void Equip(Character c)
    {
        if (StrengthBonus != 0)
        {
            //c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));

            //c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.PercentMult, this));

            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_1, StatModType.PercentMult, this));
            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_2, StatModType.PercentMult, this));
            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_3, StatModType.PercentMult, this));
        }
        if (BullshitBonus != 0)
        {
            //c.Thiccness.AddModifier(new StatModifier(BullshitBonus, StatModType.Flat, this));
           
            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_1, StatModType.PercentAdd, this));
            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_2, StatModType.PercentAdd, this));
            c.Thiccness.AddModifier(new StatModifier(BonusPercentage_3, StatModType.PercentAdd, this));

        }
        if (IDKBonus != 0)
        {
            //c.DingdongSize.AddModifier(new StatModifier(IDKBonus, StatModType.Flat, this));
        }
        if (Like4realBonus != 0)
        {
            //c.Experience.AddModifier(new StatModifier(Like4realBonus, StatModType.Flat, this));
        }


    }
    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Thiccness.RemoveAllModifiersFromSource(this);
        c.DingdongSize.RemoveAllModifiersFromSource(this);
        c.Experience.RemoveAllModifiersFromSource(this);
    }
}
