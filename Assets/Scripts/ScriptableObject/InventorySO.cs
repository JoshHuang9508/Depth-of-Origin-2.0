using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        //event
        public event Action<Dictionary<int, InventorySlot>> OnInventoryUpdated;

        //inventory item ist
        [SerializeField] private List<InventorySlot> inventorySlots;

        //size
        [field: SerializeField] public int Size { get; private set; } = 10;



        public void Initialize()
        {
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < Size; i++)
            {
                inventorySlots.Add(InventorySlot.EmptySlot());
            }
        }

        public bool IsInventoryFull(InventorySO inventory, ItemSO item)
        {
            foreach (InventorySlot inventorySlot in inventory.inventorySlots)
            {
                if (inventorySlot.item == null || (inventorySlot.item.ID == item.ID && inventorySlot.quantity < inventorySlot.item.maxStackSize))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<int, InventorySlot> GetCurrentInventoryState()
        {
            Dictionary<int, InventorySlot> returnValue = new();
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].IsEmpty)
                {
                    continue;
                }
                returnValue[i] = inventorySlots[i];
            }
            return returnValue;
        }

        public InventorySlot GetItemAt(int itemIndex)
        {
            return inventorySlots[itemIndex];
        }

        public void SwapItems(int index1, int index2)
        {
            InventorySlot tempItem = inventorySlots[index1];
            inventorySlots[index1] = inventorySlots[index2];
            inventorySlots[index2] = tempItem;

            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }

        //remove item form inventory. return those items that were removed.
        public InventorySlot RemoveItem(int index, int amount = -1)
        {
            if (inventorySlots.Count > index)
            {
                int newAmount = (amount == -1) ? 0 : inventorySlots[index].quantity - amount;
                int difAmount = (amount == -1) ? inventorySlots[index].quantity : amount;

                InventorySlot returnSlot = new() { item = inventorySlots[index].item, quantity = difAmount };
                inventorySlots[index] = (newAmount <= 0) ? InventorySlot.EmptySlot() : inventorySlots[index].ChangeQuantity(newAmount);

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                return returnSlot;
            }

            return InventorySlot.EmptySlot();
        }

        //add item to another inventory at a specific slot. return those items that can't be added in.
        public InventorySlot AddItemTo(InventorySlot inventorySlot, int index)
        {
            return AddItemTo(inventorySlot.item, inventorySlot.quantity, index);
        }

        //add item to inventory at a specific slot. return those items that can't be added in.
        public InventorySlot AddItemTo(ItemSO item, int quantity, int index)
        {
            for (; quantity > 0; quantity--)
            {
                bool isSlotFull = true;

                if (inventorySlots[index].item == null || (inventorySlots[index].item.ID == item.ID && inventorySlots[index].quantity < inventorySlots[index].item.maxStackSize))
                {
                    inventorySlots[index] = new() { item = item, quantity = inventorySlots[index].quantity + 1 };
                    isSlotFull = false;
                }

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                if (isSlotFull) return new() { item = item, quantity = quantity };
                else continue;
            }

            return InventorySlot.EmptySlot();
        }

        //add item to another inventory. return those items that can't be added in.
        public InventorySlot AddItem(InventorySlot inventorySlot)
        {
            return AddItem(inventorySlot.item, inventorySlot.quantity);
        }

        //add item to inventory. return those items that can't be added in.
        public InventorySlot AddItem(ItemSO item, int quantity)
        {
            for (; quantity > 0; quantity--)
            {
                bool isInventoryFull = true;

                for (int index = 0; index < inventorySlots.Count; index++)
                {
                    if (inventorySlots[index].item == null || (inventorySlots[index].item.ID == item.ID && inventorySlots[index].quantity < inventorySlots[index].item.maxStackSize))
                    {
                        inventorySlots[index] = new() { item = item, quantity = inventorySlots[index].quantity + 1 };
                        isInventoryFull = false;
                        break;
                    }
                }

                OnInventoryUpdated?.Invoke(GetCurrentInventoryState());

                if (isInventoryFull) return new() { item = item, quantity = quantity };
                else continue;
            }

            return InventorySlot.EmptySlot();
        }
    }

    [Serializable]
    public struct InventorySlot
    {
        public ItemSO item;
        public int quantity;

        public readonly bool IsEmpty => item == null;

        public InventorySlot ChangeQuantity(int newQuantity) => new()
        {
            item = this.item,
            quantity = newQuantity,
        };
        public static InventorySlot EmptySlot() => new()
        {
            item = null,
            quantity = 0,
        };
    }
}

