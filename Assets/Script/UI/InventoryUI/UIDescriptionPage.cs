using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using UnityEngine.Tilemaps;

namespace Inventory.UI
{
    public class UIDescriptionPage : MonoBehaviour
    {
        [Header("Settings")]
        public ActionType actionType;

        [Header("Object Reference")]
        public ItemActionPanel actionPanel;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;


        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            title.gameObject.SetActive(false);
            description.gameObject.SetActive(false);
        }

        public void SetDescription(ItemSO item)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = item.Image;
            

            title.gameObject.SetActive(true);
            title.text = item.Name;
            switch (item.Rarity)
            {
                case Rarity.Common:
                    title.outlineColor = new Color(255, 255, 255, 255);
                    break;
                case Rarity.Uncommon:
                    title.outlineColor = new Color(255, 255, 0, 255);
                    break;
                case Rarity.Rare:
                    title.outlineColor = new Color(0, 255, 0, 255);
                    break;
                case Rarity.Exotic:
                    title.outlineColor = new Color(0, 255, 255, 255);
                    break;
                case Rarity.Mythic:
                    title.outlineColor = new Color(255, 0, 255, 255);
                    break;
                case Rarity.Legendary:
                    title.outlineColor = new Color(255, 0, 0, 255);
                    break;
            }


            description.gameObject.SetActive(true);
            string descriptionText= "";
            try
            {
                var weapon = (WeaponSO)item;
                descriptionText =
                    $"- ATK Damage : {weapon.weaponDamage}\n" +
                    $"- ATK CD : {weapon.attackCooldown}s\n" +
                    $"- Knockback : {weapon.knockbackForce}\n" +
                    $"- Stunned : {weapon.knockbackTime}s\n" +
                    $"\n" +
                    $"When equipped :\n" +
                    (weapon.E_maxHealth == 0 ? "" : $"- Max HP + {weapon.E_maxHealth}\n" ) +
                    (weapon.E_strength == 0 ? "" : $"- Strength + {weapon.E_strength}\n" ) +
                    (weapon.E_defence == 0 ? "" : $"- Defence + {weapon.E_defence}\n" ) +
                    (weapon.E_walkSpeed == 0 ? "" : $"- Walk SPD + {weapon.E_walkSpeed}%\n" ) +
                    (weapon.E_critRate == 0 ? "" : $"- Crit Rate + {weapon.E_critRate}%\n" ) +
                    (weapon.E_critDamage == 0 ? "" : $"- Crit DMG + {weapon.E_critDamage}%\n" );
            }
            catch { }
            try
            {
                var edibleItem = (EdibleItemSO)item;
                descriptionText =
                    $"After consumed  :\n" +
                    (edibleItem.E_heal == 0 ? "" : $"- HP + {edibleItem.E_heal}\n" ) +
                    (edibleItem.E_maxHealth == 0 ? "" : $"- Max HP + {edibleItem.E_maxHealth}\n" ) +
                    (edibleItem.E_strength == 0 ? "" : $"- Strength + {edibleItem.E_strength}\n" ) +
                    (edibleItem.E_defence == 0 ? "" : $"- Defence + {edibleItem.E_defence}\n" ) +
                    (edibleItem.E_walkSpeed == 0 ? "" : $"- Walk SPD + {edibleItem.E_walkSpeed}%\n" ) +
                    (edibleItem.E_critRate == 0 ? "" : $"- Crit Rate + {edibleItem.E_critRate}%\n" ) +
                    (edibleItem.E_critDamage == 0 ? "" : $"- Crit DMG + {edibleItem.E_critDamage}%\n" ) +
                    (edibleItem.effectTime <= 0.5 ? "" : $"- Effect Time : {edibleItem.effectTime}s\n");
            }
            catch { }
            try
            {
                var equipment = (EquippableItemSO)item;
                descriptionText =
                    $"When equipped :\n" +
                    (equipment.E_maxHealth == 0 ? "" : $"- Max HP + {equipment.E_maxHealth}\n" ) +
                    (equipment.E_strength == 0 ? "" : $"- Strength + {equipment.E_strength}\n" ) +
                    (equipment.E_defence == 0 ? "" : $"- Defence + {equipment.E_defence}\n" ) +
                    (equipment.E_walkSpeed == 0 ? "" : $"- Walk SPD + {equipment.E_walkSpeed}%\n" ) +
                    (equipment.E_critRate == 0 ? "" : $"- Crit Rate + {equipment.E_critRate}%\n" ) +
                    (equipment.E_critDamage == 0 ? "" : $"- Crit DMG + {equipment.E_critDamage}%\n" ) ;
            }
            catch { }

            description.text = item.Description + $"\n\n{descriptionText}";
        }
    }
}