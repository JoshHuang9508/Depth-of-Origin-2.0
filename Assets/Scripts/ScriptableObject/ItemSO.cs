using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public abstract class ItemSO : ScriptableObject
    {
        [Header("Basic")]
        public string Name;
        [TextArea] public string Description;
        public Rarity rarity;

        [Header("Attributes")]
        public int sellPrice;
        public int buyPrice;
        public bool isStackable = true;
        public bool isStorable = true;
        public int maxStackSize = 1;

        [Header("References")]
        public Sprite Image;

        public int ID => GetInstanceID();

        public void EquipObject(int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            player?.SetEquipment(index);
        }

        public void UnequipObject(int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            player?.UnEquipment(index);
        }

        public void ConsumeObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); ;
            player?.SetEffection((PotionSO)inventory.GetItemAt(index).item);
            inventory.RemoveItem(index, 1);
        }

        public void SellObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            player?.ModifyCoin(sellPrice);
            inventory.RemoveItem(index, 1);
        }

        public void BuyObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            if (player.coins < buyPrice)
            {
                Debug.Log("You don't have enough money!"); // Chnage this to a UI message
            }
            else
            {
                player?.ModifyCoin(-buyPrice);
                player.backpackData.AddItem(inventory.GetItemAt(index));
            }
        }

        public void DropItem(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            InventorySlot inventorySlot = inventory.RemoveItem(index, -1);
            ItemDropper.Drop(player.transform.position, inventorySlot.item, inventorySlot.quantity);
        }
    }

    public enum Rarity
    {
        Common, Uncommon, Rare, Exotic, Mythic, Legendary
    }

    public interface IDestoryable
    {

    }

    public interface IEquipable
    {
        void EquipObject(int index);
    }

    public interface IUnequipable
    {
        void UnequipObject(int index);
    }

    public interface IConsumeable
    {
        void ConsumeObject(InventorySO inventory, int index);
    }

    public interface ISellable
    {
        void SellObject(InventorySO inventory, int index);
    }

    public interface IBuyable
    {
        void BuyObject(InventorySO inventory, int index);
    }

    public interface IDroppable
    {
        void DropItem(InventorySO inventory, int index);
    }
}




