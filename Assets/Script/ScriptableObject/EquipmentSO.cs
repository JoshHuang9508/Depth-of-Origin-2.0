using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new equipment", menuName = "Items/Equipment")]
public class EquipmentSO : ItemSO, IEquipable, IDestoryable, ISellable, IBuyable, IUnequipable, IDroppable
{
    [Header("Basic")]
    public EquipmentType equipmentType;

    [Header("Effection")]
    public float E_maxHealth;
    public float E_strength;
    public float E_walkSpeed;
    public float E_defence;
    public float E_critRate;
    public float E_critDamage;
}