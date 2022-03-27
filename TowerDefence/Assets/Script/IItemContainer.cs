public interface IItemContainer 
{
    //bool ContainItem(Items items);
    int ItemCount(string itemID);
    Items RemoveItem(string itemID);
    bool RemoveItem(Items items);
    bool CanAddItem(Items items,int amount=1);
    bool AddItem(Items items);
    void Clear();
}
