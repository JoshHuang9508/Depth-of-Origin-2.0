using Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "new weapon", menuName = "Items/Weapon")]
public class WeaponSO : ItemSO, IDestoryable, ISellable, IBuyable, IDroppable
{
    [Header("Attributes")]
    public WeaponType weaponType;
    public float weaponDamage = 1f;
    public float attackSpeed = 1f;
    public float knockbackForce;
    public float knockbackTime;

    [Header("Effection")]
    public float E_maxHealth;
    public float E_strength;
    public float E_walkSpeed;
    public float E_defence;
    public float E_critRate;
    public float E_critDamage;

    [Header("Reference")]
    public ProjectileSO projectile;
    // public Animator animator;

    [Header("Audio")]
    public AudioClip useSound;


}