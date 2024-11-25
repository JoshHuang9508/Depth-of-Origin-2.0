using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UserInterface;

public class Tradable : MonoBehaviour
{
    [Header("Attributes")]
    public List<InventorySlot> shopGoodsList = new();

    [Header("Data")]
    [SerializeField] private InventorySO shopData;

    //Runtime data
    private PlayerBehaviour player;

    private void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by Tradable.cs)");
        }
    }

    public void ToggleShop()
    {
        if (player != null)
        {
            player.ToggleShopUI();
        }
    }

    public void CloseShop()
    {
        if (player != null)
        {
            player.ToggleShopUI(PlayerBehaviour.UIOption.close);
        }
    }
}
