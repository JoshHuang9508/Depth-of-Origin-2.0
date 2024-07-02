using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

public class ItemDropper : MonoBehaviour
{
    [Header("Object Reference")]
    public GameObject itemModel;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    public void DropCoins(List<Coins> coins = null)
    {
        if (coins.Count == 0)
        {
            return;
        }

        foreach (Coins coin in coins)
        {
            for(int i = 0; i < coin.amount; i++)
            {
                var dropCoin = Instantiate(itemModel, transform.position, Quaternion.identity, transform.parent);

                dropCoin.GetComponent<DropItemInitialize>().InitialDropItem(dropCoin, coin.coin, coin.amount, 100);
            }
        }
    }

    public void DropItems(List<Lootings> lootings = null)
    {
        if (lootings.Count == 0)
        {
            return;
        }

        foreach (Lootings looting in lootings)
        {
            if(Random.Range(0, 100) < looting.chance)
            {
                for(int i = 0; i < looting.quantity; i++)
                {
                    var dropItem = Instantiate(itemModel, transform.position, Quaternion.identity, transform.parent);

                    dropItem.GetComponent<DropItemInitialize>().InitialDropItem(dropItem, looting.looting, looting.quantity, 3);
                }
            }
        }
    }

    public void DropItem(InventoryItem inventoryItem)
    {
        DropItem(inventoryItem.item, inventoryItem.quantity);
    }

    public void DropItem(ItemSO item, int quantity = 1)
    {
        for (int i = 0; i < quantity; i++)
        {
            var dropItem = Instantiate(itemModel, transform.position, Quaternion.identity, transform.parent);

            dropItem.GetComponent<DropItemInitialize>().InitialDropItem(dropItem, item, 1, 3);
        }
    }

    public void DropWrackages(List<GameObject> wreckages) 
    {
        if (wreckages.Count == 0)
        {
            return;
        }

        foreach (GameObject wreckage in wreckages)
        {
            var Wreckage = Instantiate(wreckage, transform.position, Quaternion.identity, transform.parent);

            Wreckage.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        }
    }
}

[System.Serializable]
public class Lootings
{
    public ItemSO looting;
    public int quantity;
    public float chance;

    public Lootings(ItemSO item, float chance, int quantity = 1)
    {
        this.looting = item;
        this.quantity = quantity;
        this.chance = chance;
    }
}

[System.Serializable]
public class Coins
{
    public CoinSO coin;
    public int amount;

    public Coins(CoinSO coin, int amount)
    {
        this.coin = coin;
        this.amount = amount;
    }
}
