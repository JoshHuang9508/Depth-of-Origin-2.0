using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentPage : MonoBehaviour
{
    [Header("Settings")]
    public ActionType actionType;
    [SerializeField] private bool isDragable;

    [Header("Dynamic Data")]
    [SerializeField] private List<UIItemSlot> itemSlotList = new();
    [SerializeField] private int currentDraggedItemIndex = -1;

    [Header("Object Reference")]
    public InventorySO equipmentData;
    [SerializeField] private UIInterface UIinterface;
    [SerializeField] private UIItemSlot itemSlot;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private TMP_Text healthText, strengthText, moveSpeedText, defenceText, critRateText, critDamageText;


    private void Awake()
    {
        UIinterface = GetComponentInParent<UIInterface>();

        InitializeSlot();
    }

    private void OnEnable()
    {
        HandleSlotUpdate(equipmentData.GetCurrentInventoryState());
    }

    private void Update()
    {
        SetPlayerStats(GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>());
    }

    public void InitializeSlot()
    {
        foreach (UIItemSlot _itemSlot in itemSlotList)
        {
            _itemSlot.OnItemClicked += HandleItemSelection;
            _itemSlot.OnItemBeginDrag += HandleBeginDrag;
            _itemSlot.OnItemEndDrag += HandleEndDrag;
            _itemSlot.OnItemDroppedOn += HandleSwap;
        }

        equipmentData.OnInventoryUpdated += HandleSlotUpdate;
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
        InventoryItem inventoryItem = equipmentData.GetItemAt(index);

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
            equipmentData.SwapItems(currentDraggedItemIndex, index);
            HandleItemSelection(_itemSlot);
        }
    }

    public void HandleItemSelection(UIItemSlot _itemSlot)
    {
        int index = itemSlotList.IndexOf(_itemSlot);
        InventoryItem inventoryItem = equipmentData.GetItemAt(index);

        if (index != -1 && !inventoryItem.IsEmpty)
        {
            Deselect();
            itemSlotList[index].Select();

            UIinterface.SetDescription(inventoryItem.item, actionType);
            UIinterface.SetActionBotton(equipmentData, index, actionType);
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

    public void SetPlayerStats(PlayerBehaviour player)
    {
        healthText.text = player.MaxHealth.ToString();
        strengthText.text = player.Strength.ToString();
        moveSpeedText.text = player.WalkSpeed.ToString();
        defenceText.text = player.Defence.ToString();
        critRateText.text = player.CritRate.ToString();
        critDamageText.text = player.CritDamage.ToString();
    }
}
