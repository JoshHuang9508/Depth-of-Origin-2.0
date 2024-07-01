using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponSO;

namespace Inventory.Model
{
    public abstract class ItemSO : ScriptableObject
    {
        [Header("Basic Data")]
        public string Name;
        [TextArea] public string Description;

        [Header("Setting")]
        public bool IsStackable;
        public bool isStorable = true;
        public int MaxStackSize = 1;
        public Rarity Rarity;
        public int sellPrice, buyPrice;

        [Header("Reference")]
        public Sprite Image;


        public int ID => GetInstanceID();



        public bool EquipObject(int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

            if (player != null)
            {
                player.SetEquipment(index);
            }
            return false;
        }

        public bool UnequipObject(int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

            if (player != null)
            {
                player.UnEquipment(index);
            }
            return false;
        }

        public bool ConsumeObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); ;

            if (player != null)
            {
                player.SetEffection((EdibleItemSO)inventory.GetItemAt(index).item);
                inventory.RemoveItem(index, 1);
            }
            return false;
        }

        public bool SellObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            if (player != null)
            {
                player.CoinAmount += sellPrice;
                inventory.RemoveItem(index, 1);
            }
            return false;
        }

        public bool BuyObject(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            if (player != null)
            {
                if(player.CoinAmount < buyPrice)
                {
                    Debug.Log("You don't have enough money!");
                }
                else
                {
                    player.CoinAmount -= buyPrice;
                    player.inventoryData.AddItem(inventory.GetItemAt(index));
                }
            }
            return false;
        }

        public bool DropItem(InventorySO inventory, int index)
        {
            PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            if (player != null)
            {
                player.DropItem(inventory, index);
            }
            return false;
        }

        
    }

    public enum Rarity
    {
        Common, Uncommon, Rare, Exotic, Mythic, Legendary 
    }

    public interface IDestoryableItem
    {

    }

    public interface IEquipable
    {
        bool EquipObject(int index);
    }

    public interface IUnequipable
    {
        bool UnequipObject(int index);
    }

    public interface IConsumeable
    {
        bool ConsumeObject(InventorySO inventory, int index);
    }

    public interface ISellable
    {
        bool SellObject(InventorySO inventory, int index);
    }

    public interface IBuyable
    {
        bool BuyObject(InventorySO inventory, int index);
    }

    public interface IDroppable
    {
        bool DropItem(InventorySO inventory, int index);
    }
}




