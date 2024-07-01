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

                dropCoin.GetComponent<Pickable>().PickableSetup(coin.coins, 1, 100);

                dropCoin.GetComponent<DropItemSetup>().InventoryItem = coin.coins;

                dropCoin.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2, 2f) * 10, Random.Range(-2f, 2f) * 10);
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
            if(Random.Range(0, 100) < looting.chances)
            {
                for(int i = 0; i < looting.quantity; i++)
                {
                    var dropItem = Instantiate(itemModel, transform.position, Quaternion.identity, transform.parent);

                    dropItem.GetComponent<Pickable>().PickableSetup(looting.lootings);

                    dropItem.GetComponent<DropItemSetup>().InventoryItem = looting.lootings;

                    dropItem.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2, 2f) * 10, Random.Range(-2f, 2f) * 10);
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

            dropItem.GetComponent<Pickable>().PickableSetup(item);

            dropItem.GetComponent<DropItemSetup>().InventoryItem = item;

            dropItem.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2, 2f) * 10, Random.Range(-2f, 2f) * 10);
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
    public ItemSO lootings;
    public float chances;
    public int quantity;

    public Lootings(ItemSO item, float chance, int num = 1)
    {
        lootings = item;
        chances = chance;
        quantity = num;
    }
}

[System.Serializable]
public class Coins
{
    public CoinSO coins;
    public int amount;

    public Coins(CoinSO coin, int num)
    {
        coins = coin;
        amount = num;
    }
}
