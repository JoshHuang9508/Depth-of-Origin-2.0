using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new potion", menuName = "Items/Potion")]
public class PotionSO : ItemSO, IConsumeable, IEquipable, IDestoryableItem, ISellable, IBuyable, IUnequipable, IDroppable
{
    [Header("Effection")]
    public float E_heal;
    public float E_maxHealth;
    public float E_strength;
    public float E_walkSpeed;
    public float E_defence;
    public float E_critRate;
    public float E_critDamage;
    public float effectTime;
}