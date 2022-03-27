using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemSlot : BaseItemSlot, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public event Action<BaseItemSlot> onBeginDragEvent;
    public event Action<BaseItemSlot> onDragEvent;
    public event Action<BaseItemSlot> onEndDragEvent;
    public event Action<BaseItemSlot> onDropEvent;

    private bool isDragging;
    private Color dragColor = new Color(1, 1, 1, 0.5f);

    public override bool CanAddStack(Items item, int amount = 1)
    {
        return base.CanAddStack(item, amount) && Amount + amount <= item.maximumStacks;
    }
    public override bool CanReceiveItem(Items item)
    {
        return true;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (isDragging)
            OnEndDrag(null);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (Item != null)
            image.color = dragColor;

        if (onBeginDragEvent != null)
            onBeginDragEvent(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (onDragEvent != null)
            onDragEvent(this);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (Item != null)
            image.color = normalColor;
        if (onEndDragEvent != null)
            onEndDragEvent(this);
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (onDropEvent != null)
            onDropEvent(this);
    }
}
