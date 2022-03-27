using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public Items item;
    [Range (1,1000)]
    public int amount;
}
[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemAmount> materials;
    public List<ItemAmount> results;

    public bool CanCraft(IItemContainer itemContainer)
    {
        return HasMaterial(itemContainer) && HasSpace(itemContainer) ;
    }

    private bool HasMaterial(IItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in materials)
        {
            if (itemContainer.ItemCount(itemAmount.item.ID) < itemAmount.amount)
            {
                Debug.LogWarning("not enough material");
                return false;
            }
        }
        return true;
    }
    private bool HasSpace(IItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in results)
        {
            if(!itemContainer.CanAddItem(itemAmount.item,itemAmount.amount))
            {
                Debug.LogWarning("Full Inventory");
                return false;
            }
        }
        return true;
    }
    public void Craft(IItemContainer itemContainer)
    {
        if (CanCraft(itemContainer))
        {
            Removematerial(itemContainer);
            AddResult(itemContainer);
        }
    }
    private void AddResult(IItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in results)
        {
            for (int i = 0; i < itemAmount.amount; i++)
            {
                itemContainer.AddItem(itemAmount.item.GetCopy());
            }
        }
    }
    private void Removematerial(IItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in materials)
        {
            for (int i = 0; i < itemAmount.amount; i++)
            {
                Items oldItems = itemContainer.RemoveItem(itemAmount.item.ID);
                oldItems.Destroy();
            }
        }
    }
}


