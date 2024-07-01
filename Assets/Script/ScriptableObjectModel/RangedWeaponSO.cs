using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "new ranged weapon", menuName = "Items/Weapon/Ranged Weapon")]
public class RangedWeaponSO : WeaponSO, IEquipable, IUnequipable
{
    [Header("Object Reference")]
    public GameObject projectileObject;

    [Header("Projectile Settings")]
    public ShootingType shootingType;
    public float flySpeed;
    public int splitAmount = 1;

    public enum ShootingType
    {
        Single, Split
    }
}
