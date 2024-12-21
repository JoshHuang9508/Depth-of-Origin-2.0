using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Pickable : MonoBehaviour
{
    [Header("Attributes")]
    public int quantity = 1;
    public float pickupDistance;

    [Header("Item")]
    [SerializeField] private ItemSO item;

    [Header("Reference")]
    [SerializeField] private InventorySO inventoryData;

    //Runtime data
    private PlayerBehaviour player;
    private bool canBePicked = false;
    private bool playerInRange = false;
    private bool isInventoryFull = false;


    void Start()
    {
        StartCoroutine(Pickup_delay());
    }

    void Update()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerBehaviour>() ?? null;

        CheckPlayerInRange();
        CheckInventoryFull();

        if (player.isActive && playerInRange && canBePicked && (!item.isStorable || !isInventoryFull))
        {
            MoveTowardPlayer();
        }
    }

    public void PickableSetup(ItemSO item, int quantity = 1, float pickupDistance = 3)
    {
        this.item = item;
        this.quantity = quantity;
        this.pickupDistance = pickupDistance;
    }

    private void CheckPlayerInRange()
    {
        playerInRange = Vector2.Distance(player.transform.position, this.transform.position) < pickupDistance;
    }

    private void CheckInventoryFull()
    {
        isInventoryFull = player.backpackData.IsInventoryFull(player.backpackData, item);
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 30 * Time.deltaTime);

        if (Vector2.Distance(this.transform.position, player.transform.position) <= 0.2)
        {
            if (item is CoinSO coin)
            {
                player.ModifyBalance(coin.amount);
            }
            else if (item is KeySO key)
            {
                player.keyData.AddItem(item, quantity);
            }
            else
            {
                player.backpackData.AddItem(item, quantity);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator Pickup_delay()
    {
        yield return new WaitForSeconds(1.5f);
        canBePicked = true;
    }
}
