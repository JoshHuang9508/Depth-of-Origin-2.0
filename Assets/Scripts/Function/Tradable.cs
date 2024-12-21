using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class Tradable : MonoBehaviour
{
    [Header("Data")]
    public InventorySO shopData;

    //Runtime data
    private PlayerBehaviour player;

    private void Update()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }
    public void ToggleShop()
    {
        player.shopData = shopData;
        player.ToggleShopUI();
    }
    public void CloseShop()
    {
        player.ToggleShopUI(PlayerBehaviour.UIOption.close);
    }
}
