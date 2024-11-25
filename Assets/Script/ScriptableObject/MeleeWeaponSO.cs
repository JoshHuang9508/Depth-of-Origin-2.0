using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "new melee weapon", menuName = "Items/Weapon/Melee Weapon")]
public class MeleeWeaponSO : WeaponSO, IEquipable, IUnequipable
{
    [Header("References")]
    public GameObject weaponObject;

    [Header("Melee Weapon Setting")]
    public float attackSpeed = 1f;
}
