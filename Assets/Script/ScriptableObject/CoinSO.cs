using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new coin", menuName = "Items/Coin")]
public class CoinSO : ItemSO
{
    [Header("Setting")]
    public int coinAmount;
    public RuntimeAnimatorController runtimeAnimatorController;
}
