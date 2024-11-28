using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class ItemDropper : MonoBehaviour
{
    [Header("References")]
    public GameObject itemModel;

    // Static instance
    static ItemDropper GItemDropper;

    private void Start()
    {
        GItemDropper = this;
    }
    public static void Drop(Vector2 position, List<Lootings> lootings = null)
    {
        Transform itemTransform = GameObject.FindWithTag("Item").transform;

        if (lootings.Count == 0) return;

        foreach (Lootings looting in lootings)
        {
            if (Random.Range(0, 100) > looting.chance) continue;

            for (int i = 0; i < looting.quantity; i++)
            {
                var dropItem = Instantiate(GItemDropper.itemModel, position, Quaternion.identity, itemTransform);
                dropItem.GetComponent<DroppedItem>().Initialize(dropItem, looting.item, looting.quantity, 3);
            }
        }
    }
    public static void Drop(Vector2 position, ItemSO item, int quantity = 1)
    {
        Transform itemTransform = GameObject.FindWithTag("Item").transform;

        for (int i = 0; i < quantity; i++)
        {
            var dropItem = Instantiate(GItemDropper.itemModel, position, Quaternion.identity, itemTransform);
            dropItem.GetComponent<DroppedItem>().Initialize(dropItem, item, 1, 3);
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
    public ItemSO item;
    public int quantity;
    public float chance = 100;

    public Lootings(ItemSO _item, int _quantity = 1, float _chance = 100)
    {
        item = _item;
        quantity = _quantity;
        chance = _chance;
    }
}
