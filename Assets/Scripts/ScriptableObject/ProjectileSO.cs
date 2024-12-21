using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "new projectile", menuName = "Items/Projectile")]
public class ProjectileSO : ItemSO, IDestoryable, ISellable, IBuyable, IDroppable
{
    [Header("Attributes")]
    public float speed = 100f;
    public float range = 10f;
}