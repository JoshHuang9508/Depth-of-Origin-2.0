using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image meleeWeaponImage, rangedWeaponImage, potionImage, meleeWeaponBorder, rangedWeaponBorder;
    [SerializeField] private TMP_Text potionAmountText;
    [SerializeField] private PlayerBehaviour player;

    private void Update()
    {
        if (player != null)
        {
            SetDisplay();
        }
    }

    private void SetDisplay()
    {
        meleeWeaponImage.sprite = player.equipmentData.GetItemAt(3).item != null ? player.equipmentData.GetItemAt(3).item.Image : null;
        meleeWeaponImage.color = player.equipmentData.GetItemAt(3).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        rangedWeaponImage.sprite = player.equipmentData.GetItemAt(4).item != null ? player.equipmentData.GetItemAt(4).item.Image : null;
        rangedWeaponImage.color = player.equipmentData.GetItemAt(4).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        potionImage.sprite = player.equipmentData.GetItemAt(5).item != null ? player.equipmentData.GetItemAt(5).item.Image : null;
        potionImage.color = player.equipmentData.GetItemAt(5).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        potionAmountText.text = player.equipmentData.GetItemAt(5).quantity != 0 ? player.equipmentData.GetItemAt(5).quantity.ToString() : "";
    }

    public void SetEquipmentDisplay(int index)
    {
        switch (index)
        {
            case 0:
                meleeWeaponBorder.color = new Color(190, 0, 0, 0);
                rangedWeaponBorder.color = new Color(190, 0, 0, 0);
                break;
            case 1:
                meleeWeaponBorder.color = new Color(190, 0, 0, 255);
                rangedWeaponBorder.color = new Color(190, 0, 0, 0);
                break;
            case 2:
                meleeWeaponBorder.color = new Color(190, 0, 0, 0);
                rangedWeaponBorder.color = new Color(190, 0, 0, 255);
                break;
        }
    }
}
