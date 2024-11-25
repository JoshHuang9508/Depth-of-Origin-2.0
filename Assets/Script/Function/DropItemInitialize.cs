using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropItemInitialize : MonoBehaviour
{
    [Header("Attributes")]
    public ItemSO item;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject backgroundLightObject;
    [SerializeField] private Light2D spriteLight2D;
    [SerializeField] private Light2D backgroundLight2D;
    [SerializeField] private Animator animator;

    //Runtime data
    private bool isIntialed = false;

    private void Update()
    {
        if (isIntialed)
        {
            SetSpriteLight();
        }
    }

    public void InitialDropItem(GameObject gameObject, ItemSO item, int quantity, float pickupDistant)
    {
        this.item = item;
        if (item is CoinSO coin && coin.runtimeAnimatorController != null) animator.runtimeAnimatorController = coin.runtimeAnimatorController;
        SetSpriteLight();
        SetSpriteImage();
        SetBackgroundLight();
        gameObject.GetComponent<Pickable>().PickableSetup(item, quantity, pickupDistant);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2, 2f) * 10, Random.Range(-2f, 2f) * 10);

        isIntialed = true;
    }

    private void SetSpriteImage()
    {
        spriteRenderer.sprite = item.Image;
    }

    private void SetSpriteLight()
    {
        spriteLight2D.lightCookieSprite = item.Image;
    }

    private void SetBackgroundLight()
    {
        switch (item.Rarity)
        {
            case Rarity.Common:
                backgroundLight2D.color = new Color(0.8f, 0.8f, 0.8f);
                break;
            case Rarity.Uncommon:
                backgroundLight2D.color = new Color(1, 1, 0);
                break;
            case Rarity.Rare:
                backgroundLight2D.color = new Color(0, 1, 0);
                break;
            case Rarity.Exotic:
                backgroundLight2D.color = new Color(0, 1, 1);
                break;
            case Rarity.Mythic:
                backgroundLight2D.color = new Color(2, 0, 2);
                break;
            case Rarity.Legendary:
                backgroundLight2D.color = new Color(2, 0, 0);
                break;
        }

        backgroundLightObject.transform.position = spriteRenderer.bounds.center;
    }
}
