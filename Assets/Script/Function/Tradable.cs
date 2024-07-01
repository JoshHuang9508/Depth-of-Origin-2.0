using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using Inventory.UI;

public class Tradable : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private List<InventoryItem> shopGoodsList = new();

    [Header("Object Reference")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private InventorySO shopData;

    private void Update()
    {
        try{ shopUI = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().shopUI; } catch { }
    }

    public void OpenShop()
    {
        shopData.Initialize();
        foreach (InventoryItem item in shopGoodsList)
        {
            if (item.IsEmpty)
                continue;
            shopData.AddItem(item);
        }
        shopUI.SetActive(!shopUI.activeInHierarchy);
        Time.timeScale = shopUI.activeInHierarchy ? 0 : 1;
    }

    public void CloseShop()
    {
        Time.timeScale = 1;
        shopUI.SetActive(false);
    }
}
