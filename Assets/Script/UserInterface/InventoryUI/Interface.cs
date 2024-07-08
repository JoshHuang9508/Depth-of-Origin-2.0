using Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class Interface : MonoBehaviour
    {
        [Header("Pages")]
        [SerializeField] private List<GameObject> contentPages;
        [SerializeField] private List<DescriptionPage> descriptionPages = new();
        [SerializeField] private List<ItemPage> itemPages = new();
        [SerializeField] private List<EquipmentPage> equipmentPages = new();

        [Header("Object Reference")]
        [SerializeField] private MouseFollower mouseFollower;


        private void OnDisable()
        {
            ClearDescription(ActionType.All);
            mouseFollower.Toggle(false);
        }

        private void Awake()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                contentPages.Add(transform.GetChild(i).gameObject);
            }

            foreach(GameObject contentPage in contentPages)
            {
                if (contentPage.GetComponent<ItemPage>()) itemPages.Add(contentPage.GetComponent<ItemPage>());
                if (contentPage.GetComponent<DescriptionPage>()) descriptionPages.Add(contentPage.GetComponent<DescriptionPage>());
                if (contentPage.GetComponent<EquipmentPage>()) equipmentPages.Add(contentPage.GetComponent<EquipmentPage>());
            }

            foreach(DescriptionPage descriptionPage in descriptionPages)
            {
                descriptionPage.ResetDescription();
            }
            gameObject.SetActive(false);
        }





        public void SetInventoryContent(InventorySO inventoryData, ActionType inventoryType)
        {
            foreach(ItemPage itemPage in itemPages)
            {
                if(itemPage.actionType == inventoryType || itemPage.actionType == ActionType.All)
                {
                    itemPage.inventoryData = inventoryData;
                    itemPage.HandleSlotUpdate(inventoryData.GetCurrentInventoryState());
                }
            }
            
        }

        public void SetDescription(ItemSO item, ActionType inventoryType)
        {
            foreach(DescriptionPage descriptionPage in descriptionPages)
            {
                if(descriptionPage.actionType == inventoryType || descriptionPage.actionType == ActionType.All)
                {
                    descriptionPage.SetDescription(item);
                }
            }
        }

        public void ClearDescription(ActionType inventoryType)
        {
            foreach (DescriptionPage descriptionPage in descriptionPages)
            {
                if (descriptionPage.actionType == inventoryType || descriptionPage.actionType == ActionType.All)
                {
                    descriptionPage.ResetDescription();
                    descriptionPage.actionPanel.Toggle(false);
                }
            }
        }

        public void SetActionBotton(InventorySO inventoryData, int itemIndex, ActionType inventoryType)
        {
            foreach (DescriptionPage descriptionPage in descriptionPages)
            {
                if (descriptionPage.actionType == inventoryType || descriptionPage.actionType == ActionType.All)
                {
                    InventorySlot inventoryItem = inventoryData.GetItemAt(itemIndex);
                    descriptionPage.actionPanel.Toggle(true);

                    if (!inventoryItem.IsEmpty)
                    {
                        switch (inventoryType)
                        {
                            case ActionType.BackpackItems:
                                if (inventoryItem.item is IEquipable) descriptionPage.actionPanel.AddButton("Equip", () => PerformAction(inventoryData, itemIndex, "Equip", inventoryType));
                                if (inventoryItem.item is IConsumeable) descriptionPage.actionPanel.AddButton("Consume", () => PerformAction(inventoryData, itemIndex, "Consume", inventoryType));
                                if (inventoryItem.item is IDroppable) descriptionPage.actionPanel.AddButton("Drop", () => PerformAction(inventoryData, itemIndex, "Drop", inventoryType));
                                break;
                            case ActionType.BackpackItemsInShop:
                                if (inventoryItem.item is ISellable) descriptionPage.actionPanel.AddButton($"Sell(${inventoryItem.item.sellPrice})", () => PerformAction(inventoryData, itemIndex, "Sell", inventoryType));
                                break;
                            case ActionType.ShopItems:
                                if (inventoryItem.item is IBuyable) descriptionPage.actionPanel.AddButton($"Buy(${inventoryItem.item.buyPrice})", () => PerformAction(inventoryData, itemIndex, "Buy", inventoryType));
                                break;
                            case ActionType.Equipments:
                                if (inventoryItem.item is IUnequipable) descriptionPage.actionPanel.AddButton("Unequip", () => PerformAction(inventoryData, itemIndex, "Unequip", inventoryType));
                                break;
                        }
                    }
                }
            }
        }

        public void PerformAction(InventorySO inventoryData, int itemIndex, string actionName, ActionType inventoryType)
        {
            foreach (DescriptionPage descriptionPage in descriptionPages)
            {
                if (descriptionPage.actionType == inventoryType || descriptionPage.actionType == ActionType.All)
                {
                    InventorySlot inventoryItem = inventoryData.GetItemAt(itemIndex);

                    if (!inventoryItem.IsEmpty)
                    {
                        switch (actionName)
                        {
                            case "Equip":
                                IEquipable ObjectToEquip = inventoryItem.item as IEquipable;
                                ObjectToEquip.EquipObject(itemIndex);
                                break;
                            case "Unequip":
                                IUnequipable ObjectToUnequip = inventoryItem.item as IUnequipable;
                                ObjectToUnequip.UnequipObject(itemIndex);
                                break;
                            case "Consume":
                                IConsumeable ObjectToConsume = inventoryItem.item as IConsumeable;
                                ObjectToConsume.ConsumeObject(inventoryData, itemIndex);
                                break;
                            case "Drop":
                                IDroppable ObjectToDrop = inventoryItem.item as IDroppable;
                                ObjectToDrop.DropItem(inventoryData, itemIndex);
                                ClearDescription(inventoryType);
                                break;
                            case "Sell":
                                ISellable ObjectToSell = inventoryItem.item as ISellable;
                                ObjectToSell.SellObject(inventoryData, itemIndex);
                                break;
                            case "Buy":
                                IBuyable ObjectToBuy = inventoryItem.item as IBuyable;
                                ObjectToBuy.BuyObject(inventoryData, itemIndex);
                                break;
                        }

                        if (inventoryData.GetItemAt(itemIndex).IsEmpty) ClearDescription(inventoryType);
                    }
                }
            }
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        public void DeletDraggedItem()
        {
            mouseFollower.Toggle(false);
        }
    }

    public enum ActionType
    {
        BackpackItems, Equipments, BackpackItemsInShop, ShopItems, All
    }
}