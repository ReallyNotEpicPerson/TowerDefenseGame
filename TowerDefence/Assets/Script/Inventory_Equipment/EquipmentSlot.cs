using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public EquipmentType equipmentType;
    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = equipmentType.ToString() + " Slot ";
    }
    public override bool CanReceiveItem(Items item)
    {
        if (item == null)
            return true;
        EquippableItem equippableItem = item as EquippableItem;
        return (equippableItem != null && equippableItem.equipmentType == equipmentType);
    }
}
