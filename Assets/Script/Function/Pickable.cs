using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Pickable : MonoBehaviour
{
    [Header("Setting")]
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
        StartCoroutine(pickup_delay());
    }

    void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by pickable.cs)");
        }

        if(player != null)
        {
            inventoryData = player.backpackData;
        }

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
        isInventoryFull = inventoryData.IsInventoryFull(player.backpackData, item);
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 30 * Time.deltaTime);

        if (Vector2.Distance(this.transform.position, player.transform.position) <= 0.2)
        {
            if (item is CoinSO)
            {
                player.coinAmount += ((CoinSO)item).coinAmount;
            }
            else if (item is KeySO)
            {
                player.keyList.Add(new Key { key = (KeySO)item, quantity = 1 });
            }
            else
            {
                inventoryData.AddItem(item, quantity);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator pickup_delay()
    {
        yield return new WaitForSeconds(1.5f);
        canBePicked = true;
    }
}
