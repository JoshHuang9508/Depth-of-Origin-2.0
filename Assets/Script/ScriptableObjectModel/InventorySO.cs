using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        [SerializeField] private List<InventoryItem> inventoryItems;
        [field: SerializeField] public int Size { get; private set; } = 10;



        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public bool IsInventoryFull(InventorySO inventory, ItemSO item)
        {
            foreach (InventoryItem inventoryItem in inventory.inventoryItems)
            {
                if (inventoryItem.item == null || (inventoryItem.item.ID == item.ID && inventoryItem.quantity < inventoryItem.item.MaxStackSize))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    continue;
                }
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int index1, int index2)
        {
            InventoryItem item1 = inventoryItems[index1];
            inventoryItems[index1] = inventoryItems[index2];
            inventoryItems[index2] = item1;

            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }

        public InventoryItem RemoveItem(int index, int amount)
        {
            if (inventoryItems.Count > index)
            {
                int newAmount = (amount == -1) ? 0 : inventoryItems[index].quantity - amount;
                int difAmount = (amount == -1) ? inventoryItems[index].quantity : amount;

                InventoryItem returnItem = new() { item = inventoryItems[index].item, quantity = difAmount };
                inventoryItems[index] = (newAmount <= 0) ? InventoryItem.GetEmptyItem() : inventoryItems[index].ChangeQuantity(newAmount);

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                return returnItem;
            }

            return InventoryItem.GetEmptyItem();
        }

        public InventoryItem AddItemTo(InventoryItem inventoryItem, int index)
        {
            return AddItemTo(inventoryItem.item, inventoryItem.quantity, index);
        }

        public InventoryItem AddItemTo(ItemSO item, int quantity, int index)
        {
            for (; quantity > 0; quantity--)
            {
                bool isSlotFull = true;

                if (inventoryItems[index].item == null || (inventoryItems[index].item.ID == item.ID && inventoryItems[index].quantity < inventoryItems[index].item.MaxStackSize))
                {
                    inventoryItems[index] = new() { item = item, quantity = inventoryItems[index].quantity + 1 };
                    isSlotFull = false;
                }

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                if (isSlotFull) return new() { item = item, quantity = quantity };
                else continue;
            }

            return InventoryItem.GetEmptyItem();
        }

        public InventoryItem AddItem(InventoryItem inventoryItem)
        {
            return AddItem(inventoryItem.item, inventoryItem.quantity);
        }

        public InventoryItem AddItem(ItemSO item, int quantity)
        {
            for (; quantity > 0; quantity--)
            {
                bool isInventoryFull = true;

                for (int index = 0; index < inventoryItems.Count; index++)
                {
                    if (inventoryItems[index].item == null || (inventoryItems[index].item.ID == item.ID && inventoryItems[index].quantity < inventoryItems[index].item.MaxStackSize))
                    {
                        inventoryItems[index] = new() { item = item, quantity = inventoryItems[index].quantity + 1 };
                        isInventoryFull = false;
                        break;
                    }
                }

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                if (isInventoryFull) return new() { item = item, quantity = quantity };
                else continue;
            }

            return InventoryItem.GetEmptyItem();
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity) => new InventoryItem
        {
            item = this.item,
            quantity = newQuantity,
        };

        public static InventoryItem GetEmptyItem() => new InventoryItem
        {
            item = null,
            quantity = 0,
        };
    }
}

