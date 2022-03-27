using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : ItemContainer
{
    [SerializeField] private Items[] StartingItem;
    [SerializeField] private Transform itemsParent;

    public event Action<BaseItemSlot> onPointerEnterEvent;
    public event Action<BaseItemSlot> onPointerExitEvent;
    public event Action<BaseItemSlot> onRightClickedEvent;
    public event Action<BaseItemSlot> onBeginDragEvent;
    public event Action<BaseItemSlot> onDragEvent;
    public event Action<BaseItemSlot> onEndDragEvent;
    public event Action<BaseItemSlot> onDropEvent;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].onRightClickEvent += slot => onRightClickedEvent(slot);
            itemSlots[i].onPointerEnterEvent += slot => onPointerEnterEvent(slot);
            itemSlots[i].onPointerExitEvent += slot => onPointerExitEvent(slot);
            itemSlots[i].onBeginDragEvent += slot => onBeginDragEvent(slot);
            itemSlots[i].onEndDragEvent += slot => onEndDragEvent(slot);
            itemSlots[i].onDragEvent += slot => onDragEvent(slot);
            itemSlots[i].onDropEvent += slot=> onDropEvent(slot);
        }
        SetStartingItem();
    }
    private void OnValidate()
    {
        if (itemsParent != null)
        {
            itemSlots = new ItemSlot[itemsParent.childCount];
            for (int i = 0; i < itemsParent.childCount; i++)
            {
                itemSlots[i] = itemsParent.transform.GetChild(i).GetComponent<ItemSlot>();
            }
        }
        SetStartingItem();
    }/*
    public bool AddItem(Items item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == null || itemSlots[i].Item.ID==item.ID && itemSlots[i].Amount <item.maximumStacks)
            {
                itemSlots[i].Item = item;
                itemSlots[i].Amount++;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Items item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == item)
            {   
                itemSlots[i].Amount--;
                if (itemSlots[i].Amount == 0)
                {
                    itemSlots[i].Item = null;
                }
                return true;
            }
        }
        return false;
    }
    public Items RemoveItem(string itemID)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Items items = itemSlots[i].Item;
            if (items!=null && items.ID == itemID)
            {
                itemSlots[i].Amount--;
                if (itemSlots[i].Amount == 0)
                {
                    itemSlots[i].Item = null;
                }
                return items;
            }
        }
        return null;
    }
    
    public bool IsFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == null)
            {
                return false;
            }
        }
        return true;
    }*/
    private void SetStartingItem()
    {
        Clear();
        for (int i = 0; i < StartingItem.Length; i++)
        {
            AddItem(StartingItem[i].GetCopy());
        }
    }
    public virtual bool ContainItem(Items items)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == items)
            {
                return true;
            }
        }
        return false;
    }
    /*
    public int ItemCount(Items items)
    {
        int count = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == items)
            {
                count++;
            }
        }
        return count;
    }
    
    public int ItemCount(string itemID)
    {
        int count = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item.ID == itemID)
            {
                count++;
            }
        }
        return count;
    }*/
}
