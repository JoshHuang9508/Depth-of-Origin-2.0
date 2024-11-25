using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Name")]
    public string Name;

    [Header("Attributes")]
    public float maxHealth;
    public float walkSpeed;
    public bool haveShield;
    public float maxShieldHealth;
    public float strength;
    public float defence;
    public float critRate;
    public float critDamage;
    public float dodge;
    public float chaseField;
    public float attackField;
    public bool isBoss;
    public WalkType walkType;
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Weapon")]
    public WeaponSO weapon;

    [Header("Lootings")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("References")]
    public GameObject enemyObject;
    public int angleOffset;

    public enum Difficulty
    {
        Easy, Normal, Hard, Difficult, Extreme
    }

    public enum WalkType
    {
        Melee, Sniper, None
    }
}