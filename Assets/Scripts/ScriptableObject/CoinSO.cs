using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "new coin", menuName = "Items/Coin")]
public class CoinSO : ItemSO
{
    [Header("Attributes")]
    public int amount;

    [Header("References")]
    public RuntimeAnimatorController runtimeAnimatorController;
}
