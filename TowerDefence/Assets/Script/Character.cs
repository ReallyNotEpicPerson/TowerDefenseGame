using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Stat")]
    public CharacterStat Strength;
    public CharacterStat Thiccness;
    public CharacterStat DingdongSize;
    public CharacterStat Experience;

    [SerializeField] CraftingWindow craftingWindow;
    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentPanel equipmentPanel;
    [SerializeField] private StatPanel statPanel;
    [SerializeField] private ItemToolTips itemToolTips;

    [SerializeField] Image dragableItem;
    private BaseItemSlot dragItemSlot;

    private void OnValidate()
    {
        if (itemToolTips == null)
        {
            itemToolTips = FindObjectOfType<ItemToolTips>();
        }
    }
    private void Start()
    {
        statPanel.SetStat(Strength, Thiccness, DingdongSize, Experience);
        statPanel.UpdateStatValue();
        //set up event
        //
        inventory.onRightClickedEvent += Equip;
        equipmentPanel.onRightClickedEvent += Unequip;

        inventory.onPointerEnterEvent += ShowToolTips;
        equipmentPanel.onPointerEnterEvent += ShowToolTips;
        craftingWindow.OnPointerEnterEvent += ShowToolTips;

        inventory.onPointerExitEvent += HideToolTips;
        equipmentPanel.onPointerExitEvent += HideToolTips;
        craftingWindow.OnPointerExitEvent += HideToolTips;

        inventory.onBeginDragEvent += BeginDrag;
        equipmentPanel.onBeginDragEvent += BeginDrag;

        inventory.onDragEvent += Drag;
        equipmentPanel.onDragEvent += Drag;

        inventory.onEndDragEvent += EndDrag;
        equipmentPanel.onEndDragEvent += EndDrag;

        inventory.onDropEvent += Drop;
        equipmentPanel.onDropEvent += Drop;
    }
    private void Equip(BaseItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            Equip(equippableItem);
        }
    }
    private void Unequip(BaseItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            Unequip(equippableItem);
        }
    }
    private void ShowToolTips(BaseItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            itemToolTips.Show(equippableItem);
        }
    }
    private void HideToolTips(BaseItemSlot itemSlot)
    {
        itemToolTips.Hide();
    }
    private void BeginDrag(BaseItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            dragItemSlot = itemSlot;
            dragableItem.sprite = itemSlot.Item.icon;
            dragableItem.transform.position = Input.mousePosition;
            dragableItem.enabled = true;
        }
    }
    private void Drag(BaseItemSlot itemSlot)
    {
        if (dragableItem.enabled)
        {
            dragableItem.transform.position = Input.mousePosition;
        }
    }
    private void EndDrag(BaseItemSlot itemSlot)
    {
        dragItemSlot = null;
        dragableItem.enabled = false;
    }
    private void Drop(BaseItemSlot dropItemSlot)
    {
        if (dragItemSlot == null) { return; }

        if (dropItemSlot.CanAddStack(dragItemSlot.Item))
        {
            AddStack(dropItemSlot);
        }

        else if (dropItemSlot.CanReceiveItem(dragItemSlot.Item) && dragItemSlot.CanReceiveItem(dropItemSlot.Item))
        {
            SwapItem(dropItemSlot);
        }
    }
    private void SwapItem(BaseItemSlot dropItemSlot)
    {
        EquippableItem Dragitem = dragItemSlot.Item as EquippableItem;
        EquippableItem Dropitem = dropItemSlot.Item as EquippableItem;

        if (dragItemSlot is EquipmentSlot)
        {
            if (Dragitem != null) { Dragitem.Unequip(this); }
            if (Dropitem != null) { Dropitem.Equip(this); }
        }
        if (dropItemSlot is EquipmentSlot)
        {
            if (Dragitem != null) { Dragitem.Equip(this); }
            if (Dropitem != null) { Dropitem.Unequip(this); }
        }
        statPanel.UpdateStatValue();

        Items DraggedItem = dragItemSlot.Item;
        int DraggedItemAmount = dragItemSlot.Amount;

        dragItemSlot.Item = dropItemSlot.Item;
        dragItemSlot.Amount = dropItemSlot.Amount;

        dropItemSlot.Item = DraggedItem;
        dropItemSlot.Amount = DraggedItemAmount;
    }

    private void AddStack(BaseItemSlot dropItemSlot)
    {
        int numAddableStack = dropItemSlot.Item.maximumStacks - dropItemSlot.Amount;
        int stackToAdd = Mathf.Min(numAddableStack, dragItemSlot.Amount);
        dropItemSlot.Amount += stackToAdd;
        dragItemSlot.Amount -= stackToAdd;
    }

    public void Equip(EquippableItem item)
    {
        if (inventory.RemoveItem(item))
        {
            if (equipmentPanel.AddItem(item, out EquippableItem previousItem))
            {
                if (previousItem != null)
                {
                    inventory.AddItem(previousItem);
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValue();
                }
                item.Equip(this);
                statPanel.UpdateStatValue();
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }
    public void Unequip(EquippableItem item)
    {
        if (inventory.CanAddItem(item) && equipmentPanel.RemoveItem(item))
        {
            item.Unequip(this);
            statPanel.UpdateStatValue();
            inventory.AddItem(item);
        }
    }
}
