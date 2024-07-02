using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using Inventory.UI;

public class Tradable : MonoBehaviour
{
    [Header("Setting")]
    public List<InventoryItem> shopGoodsList = new();

    [Header("Reference")]
    [SerializeField] private InventorySO shopData;

    //Runtime data
    private PlayerBehaviour player;
    private GameObject shopUI;

    private void Update()
    {
        try
        { 
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        } 
        catch 
        {
            Debug.LogWarning("Can't find player (sent by tradable.cs)");
        }

        if(player != null)
        {
            shopUI = player.shopUI;
        }
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
