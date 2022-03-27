using System;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{    
    public EquipmentSlot[] equipmentSlot;
    [SerializeField] Transform equipmentSlotParent;

    public event Action<BaseItemSlot> onPointerEnterEvent;
    public event Action<BaseItemSlot> onPointerExitEvent;
    public event Action<BaseItemSlot> onRightClickedEvent;
    public event Action<BaseItemSlot> onBeginDragEvent;
    public event Action<BaseItemSlot> onDragEvent;
    public event Action<BaseItemSlot> onEndDragEvent;
    public event Action<BaseItemSlot> onDropEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].onRightClickEvent += slot=> onRightClickedEvent(slot);
            equipmentSlot[i].onPointerEnterEvent += slot => onPointerEnterEvent(slot);
            equipmentSlot[i].onPointerExitEvent += slot => onPointerExitEvent(slot);
            equipmentSlot[i].onBeginDragEvent += slot => onBeginDragEvent(slot);
            equipmentSlot[i].onEndDragEvent += slot => onEndDragEvent(slot);
            equipmentSlot[i].onDragEvent += slot => onDragEvent(slot);
            equipmentSlot[i].onDropEvent += slot => onDropEvent(slot);
        }
    }
    private void OnValidate()
    {
        if (equipmentSlotParent != null)
        {
            equipmentSlot = new EquipmentSlot[equipmentSlotParent.childCount];
            for (int i = 0; i < equipmentSlotParent.childCount; i++)
            {
                equipmentSlot[i] = equipmentSlotParent.transform.GetChild(i).GetComponent<EquipmentSlot>();
            }
        }
    }
    public bool AddItem(EquippableItem item, out EquippableItem previousItem)
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].equipmentType == item.equipmentType)
            {
                previousItem =(EquippableItem) equipmentSlot[i].Item;
                equipmentSlot[i].Item = item;
                equipmentSlot[i].Amount = 1;
                return true;
            }
        }
        previousItem = null;
        return false;
    }
    public bool RemoveItem(EquippableItem item)
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].Item == item)
            {
                equipmentSlot[i].Item = null;
                equipmentSlot[i].Amount = 0;
                return true;
            }
        }
        return false;
    }
}
