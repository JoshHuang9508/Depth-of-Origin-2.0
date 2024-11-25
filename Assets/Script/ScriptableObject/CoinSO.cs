using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new coin", menuName = "Items/Coin")]
public class CoinSO : ItemSO
{
    [Header("Attributes")]
    public int coinAmount;
    public RuntimeAnimatorController runtimeAnimatorController;
}
