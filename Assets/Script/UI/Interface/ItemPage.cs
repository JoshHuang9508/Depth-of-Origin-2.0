using Inventory;
using UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemPage : MonoBehaviour
{
    [Header("Attributes")]
    public ActionType actionType;
    [SerializeField] private bool isDragable;

    [Header("Status")]
    [SerializeField] private List<ItemSlot> itemSlotList = new();
    [SerializeField] private int currentDraggedItemIndex = -1;

    [Header("References")]
    public InventorySO inventoryData;
    [SerializeField] private Interface UIinterface;
    [SerializeField] private ItemSlot itemSlot;
    [SerializeField] private RectTransform contentPanel;


    private void Awake()
    {
        UIinterface = GetComponentInParent<Interface>();

        InitializeSlot(inventoryData.Size);
    }

    private void OnEnable()
    {
        HandleSlotUpdate(inventoryData.GetCurrentInventoryState());
    }

    public void InitializeSlot(int size)
    {
        for (int i = 0; i < size; i++)
        {
            ItemSlot _itemSlot = Instantiate(itemSlot, Vector3.zero, Quaternion.identity, contentPanel);
            _itemSlot.OnItemClicked += HandleItemSelection;
            _itemSlot.OnItemBeginDrag += HandleBeginDrag;
            _itemSlot.OnItemEndDrag += HandleEndDrag;
            _itemSlot.OnItemDroppedOn += HandleSwap;
            itemSlotList.Add(_itemSlot);
        }

        inventoryData.OnInventoryUpdated += HandleSlotUpdate;
    }





    public void HandleSlotUpdate(Dictionary<int, InventorySlot> inventoryState)
    {
        ResetAllItems();

        foreach (var item in inventoryState)
        {
            if (itemSlotList.Count > item.Key)
            {
                itemSlotList[item.Key].SetData(item.Value.item.Image, item.Value.quantity);
            }
        }
    }

    public void HandleBeginDrag(ItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);
        InventorySlot inventoryItem = inventoryData.GetItemAt(index);

        if (index != -1 && !inventoryItem.IsEmpty && isDragable)
        {
            HandleItemSelection(_itemSlot);
            currentDraggedItemIndex = index;
            UIinterface.CreateDraggedItem(inventoryItem.item.Image, inventoryItem.quantity);
        }
    }

    public void HandleEndDrag(ItemSlot _itemSlot)
    {
        UIinterface.DeletDraggedItem();
        currentDraggedItemIndex = -1;
    }

    public void HandleSwap(ItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);

        if (index != -1 && isDragable)
        {
            inventoryData.SwapItems(currentDraggedItemIndex, index);
            HandleItemSelection(_itemSlot);
        }
    }

    public void HandleItemSelection(ItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);
        InventorySlot inventoryItem = inventoryData.GetItemAt(index);

        if (index != -1 && !inventoryItem.IsEmpty)
        {
            Deselect();
            itemSlotList[index].Select();

            UIinterface.SetDescription(inventoryItem.item, actionType);
            UIinterface.SetActionBotton(inventoryData, index, actionType);
        }
        else
        {
            Deselect();
        }
    }





    public void Deselect()
    {
        foreach (ItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.Deselect();
        }
        UIinterface.ClearDescription(actionType);
    }

    public void ResetAllItems()
    {
        foreach (ItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.ResetData();
        }
    }
}
