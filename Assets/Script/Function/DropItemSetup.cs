using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropItemSetup : MonoBehaviour
{
    [field: SerializeField] public ItemSO InventoryItem { get; set; }

    [Header("Object Reference")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject backgroundLightObject;
    [SerializeField] private Light2D spriteLight2D;
    [SerializeField] private Light2D backgroundLight2D;
    [SerializeField] private Animator animator;

    void Start()
    {
        if(InventoryItem is CoinSO coin && coin.runtimeAnimatorController != null)
        {
            animator.runtimeAnimatorController = coin.runtimeAnimatorController;
        }
        SetSpriteImage();
        SetSpriteLight();
        SetBackgroundLight();
    }

    private void Update()
    {
        SetSpriteLight();
    }

    private void SetSpriteImage()
    {
        spriteRenderer.sprite = InventoryItem.Image;
    }

    private void SetSpriteLight()
    {
        spriteLight2D.lightCookieSprite = InventoryItem.Image;
    }

    private void SetBackgroundLight()
    {
        switch (InventoryItem.Rarity)
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
