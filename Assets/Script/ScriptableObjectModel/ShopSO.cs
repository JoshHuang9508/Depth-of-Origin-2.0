using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ShopSO : ScriptableObject
{
    [SerializeField] private List<ShopItem> shopItems;
    public event Action<Dictionary<int, ShopItem>> OnShopUpdated;
    [field: SerializeField] public int Size { get; private set; } = 10;


    public void initialize()
    {
        shopItems = new List<ShopItem>();
        for (int i = 0; i < Size; i++)
        {
            shopItems.Add(ShopItem.GetEmptyItem());
        }
    }

    public ShopItem GetItemAt(int itemIndex)
    {
        return shopItems[itemIndex];
    }



    public void AddItem(ShopItem item)
    {
        AddItem(item.item, item.quantity);
    }



    public int AddItem(ItemSO item, int quantity)
    {
        if (item.IsStackable == false)
        {
            for (int i = 0; i < shopItems.Count; i++)
            {
                while (quantity > 0 && IsInventoryFull() == false)
                {
                    quantity -= AddItemToFristFreeSlot(item, 1);
                }
            }

            OnShopUpdated?.Invoke(GetCurrentInventoryState());
            return quantity;
        }

        else
        {
            for (int i = 0; i < shopItems.Count; i++)
            {
                if (shopItems[i].IsEmpty) continue;
                if (shopItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTake = shopItems[i].item.MaxStackSize - shopItems[i].quantity;

                    if (quantity > amountPossibleToTake)
                    {
                        shopItems[i] = shopItems[i].ChangeQuantity(shopItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else
                    {
                        shopItems[i] = shopItems[i].ChangeQuantity(shopItems[i].quantity + quantity);

                        OnShopUpdated?.Invoke(GetCurrentInventoryState());
                        return quantity;
                    }
                }
            }

            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFristFreeSlot(item, newQuantity);
            }

            OnShopUpdated?.Invoke(GetCurrentInventoryState());
            return quantity;
        }
    }
    private int AddItemToFristFreeSlot(ItemSO item, int quantity)
    {
        ShopItem newItem = new ShopItem
        {
            item = item,
            quantity = quantity,
        };
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsEmpty)
            {
                shopItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    private bool IsInventoryFull()
    => shopItems.Where(item => item.IsEmpty).Any() == false;


    public Dictionary<int, ShopItem> GetCurrentInventoryState()
    {
        Dictionary<int, ShopItem> returnValue = new Dictionary<int, ShopItem>();
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsEmpty)
            {
                continue;
            }
            returnValue[i] = shopItems[i];
        }
        return returnValue;
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (shopItems.Count > itemIndex)
        {
            if (shopItems[itemIndex].IsEmpty) return;

            int temp = shopItems[itemIndex].quantity - amount;

            shopItems[itemIndex] = temp <= 0 ? ShopItem.GetEmptyItem() : shopItems[itemIndex].ChangeQuantity(temp);

            OnShopUpdated?.Invoke(GetCurrentInventoryState());
        }
    }




}

[Serializable]
public struct ShopItem
{
    public int quantity;
    public ItemSO item;

    public bool IsEmpty => item == null;

    public ShopItem ChangeQuantity(int newQuantity)
    {
        return new ShopItem
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    public static ShopItem GetEmptyItem() => new ShopItem
    {
        item = null,
        quantity = 0,
    };
}
