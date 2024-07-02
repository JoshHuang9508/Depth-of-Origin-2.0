using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIItemPage : MonoBehaviour
{
    [Header("Settings")]
    public ActionType actionType;
    [SerializeField] private bool isDragable;

    [Header("Dynamic Data")]
    [SerializeField] private List<UIItemSlot> itemSlotList = new();
    [SerializeField] private int currentDraggedItemIndex = -1;

    [Header("Object Reference")]
    public InventorySO inventoryData;
    [SerializeField] private UIInterface UIinterface;
    [SerializeField] private UIItemSlot itemSlot;
    [SerializeField] private RectTransform contentPanel;

    
    private void Awake()
    {
        UIinterface = GetComponentInParent<UIInterface>();

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
            UIItemSlot _itemSlot = Instantiate(itemSlot, Vector3.zero, Quaternion.identity, contentPanel);
            _itemSlot.OnItemClicked += HandleItemSelection;
            _itemSlot.OnItemBeginDrag += HandleBeginDrag;
            _itemSlot.OnItemEndDrag += HandleEndDrag;
            _itemSlot.OnItemDroppedOn += HandleSwap;
            itemSlotList.Add(_itemSlot);
        }

        inventoryData.OnInventoryUpdated += HandleSlotUpdate;
    }





    public void HandleSlotUpdate(Dictionary<int, InventoryItem> inventoryState)
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

    public void HandleBeginDrag(UIItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);
        InventoryItem inventoryItem = inventoryData.GetItemAt(index);

        if (index != -1 && !inventoryItem.IsEmpty && isDragable)
        {
            HandleItemSelection(_itemSlot);
            currentDraggedItemIndex = index;
            UIinterface.CreateDraggedItem(inventoryItem.item.Image, inventoryItem.quantity);
        }
    }

    public void HandleEndDrag(UIItemSlot _itemSlot)
    {
        UIinterface.DeletDraggedItem();
        currentDraggedItemIndex = -1;
    }

    public void HandleSwap(UIItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);

        if (index != -1 && isDragable)
        {
            inventoryData.SwapItems(currentDraggedItemIndex, index);
            HandleItemSelection(_itemSlot);
        }
    }

    public void HandleItemSelection(UIItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);
        InventoryItem inventoryItem = inventoryData.GetItemAt(index);

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
        foreach (UIItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.Deselect();
        }
        UIinterface.ClearDescription(actionType);
    }

    public void ResetAllItems()
    {
        foreach (UIItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.ResetData();
        }
    }
}
