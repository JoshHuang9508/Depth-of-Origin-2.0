using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentDisplay : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private Image meleeWeaponImage;
    [SerializeField] private Image rangedWeaponImage;
    [SerializeField] private Image potionImage;
    [SerializeField] private Image meleeWeaponBorder;
    [SerializeField] private Image rangedWeaponBorder;
    [SerializeField] private TMP_Text potionAmountText;
    [SerializeField] private PlayerBehaviour player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        meleeWeaponImage.sprite = player.equipmentData.GetItemAt(3).item != null ? player.equipmentData.GetItemAt(3).item.Image : null;
        meleeWeaponImage.color = player.equipmentData.GetItemAt(3).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        rangedWeaponImage.sprite = player.equipmentData.GetItemAt(4).item != null ? player.equipmentData.GetItemAt(4).item.Image : null;
        rangedWeaponImage.color = player.equipmentData.GetItemAt(4).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        potionImage.sprite = player.equipmentData.GetItemAt(5).item != null ? player.equipmentData.GetItemAt(5).item.Image : null;
        potionImage.color = player.equipmentData.GetItemAt(5).item != null ? new Color(255, 255, 255, 255) : new Color(255, 255, 255, 0);
        potionAmountText.text = player.equipmentData.GetItemAt(5).quantity != 0 ? player.equipmentData.GetItemAt(5).quantity.ToString() : "";

        switch (player.weaponControl)
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
