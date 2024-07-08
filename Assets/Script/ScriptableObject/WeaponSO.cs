using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WeaponSO : ItemSO, IDestoryableItem, ISellable, IBuyable, IDroppable
{
    [Header("Attributes")]
    public float weaponDamage = 1f;
    public float attackCooldown;
    public float knockbackForce;
    public float knockbackTime;

    [Header("Effection")]
    public float E_maxHealth;
    public float E_strength;
    public float E_walkSpeed;
    public float E_defence;
    public float E_critRate;
    public float E_critDamage;
}
