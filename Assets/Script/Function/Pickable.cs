using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Pickable : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private int quantity = 1;
    [SerializeField] private float pickupDistance;

    [Header("Object Reference")]
    [SerializeField] private ItemSO item;
    [SerializeField] private InventorySO inventoryData;
    [SerializeField] private PlayerBehaviour player;

    [Header("Dynamic Data")]
    [SerializeField] private bool pickEnabler = false;
    

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        StartCoroutine(pickup_delay());
    }

    void Update()
    {
        bool isInventoryFull = inventoryData.IsInventoryFull(player.inventoryData, item);

        if (player.behaviourEnabler && (item.isStorable ? !isInventoryFull : true) && pickEnabler && Vector2.Distance(player.transform.position, this.transform.position) < pickupDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 30 * Time.deltaTime);

            if(Vector2.Distance(this.transform.position, player.transform.position) <= 0.2)
            {
                if(item is CoinSO)
                {
                    player.CoinAmount += ((CoinSO)item).coinAmount;
                }
                else if(item is KeySO)
                {
                    player.GetKeyList.Add(new PlayerBehaviour.Key {key = (KeySO)item, quantity = 1});
                }
                else
                {
                    inventoryData.AddItem(item, quantity);
                }
                Destroy(gameObject);
            }
        }
    }

    public void PickableSetup(ItemSO item, int quantity = 1, float pickupDistance = 3)
    {
        this.item = item;
        this.quantity = quantity;
        this.pickupDistance = pickupDistance;
    }


    private IEnumerator pickup_delay()
    {
        yield return new WaitForSeconds(1.5f);
        pickEnabler = true;
    }
}
