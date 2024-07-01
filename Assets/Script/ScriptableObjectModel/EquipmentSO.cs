using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EquipmentSO : ScriptableObject
{

    public event Action<Dictionary<int, EquimentItem>> OnInventoryUpdated;
    [SerializeField] private List<EquimentItem> equipmentItems = new List<EquimentItem>();

    enum equipmentType
    {
        meleeWeapon,rangedWeapon,potions,armor,jewery,book
    };

    public EquimentItem getItem(int index)
    {
        return equipmentItems[index];
    }


    public void removeItem()
    {

    }

    public Dictionary<int, EquimentItem> GetCurrentInventoryState()
    {
        Dictionary<int, EquimentItem> returnValue = new Dictionary<int, EquimentItem>();
        for (int i = 0; i < equipmentItems.Count; i++)
        {
            if (equipmentItems[i].IsEmpty)
            {
                continue;
            }
            returnValue[i] = equipmentItems[i];
        }
        return returnValue;
    }

    public void AddItem(int itemIndex, int amount)
    {
        if (equipmentItems.Count > itemIndex)
        {
            if (equipmentItems[itemIndex].IsEmpty) return;

            int temp = equipmentItems[itemIndex].quantity + amount;

            equipmentItems[itemIndex] = temp <= 0 ? EquimentItem.GetEmptyItem() : equipmentItems[itemIndex].ChangeQuantity(temp);

            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }
}

[Serializable]
public struct EquimentItem
{
    public ItemSO item;
    public int quantity;

    public bool IsEmpty => item == null;
    public EquimentItem ChangeQuantity(int newQuantity)
    {
        return new EquimentItem
        {
            item = this.item,
            quantity = newQuantity
        };
    }

    public static EquimentItem GetEmptyItem() => new EquimentItem
    {
        item = null,
        quantity = 0,
    };
}
