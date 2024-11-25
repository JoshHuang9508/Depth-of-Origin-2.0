using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Basic")]
    public string Name;
    public bool isBoss;

    [Header("Attributes")]
    public float maxHealth;
    public bool haveShield;
    public float maxShieldHealth;
    public float walkSpeed;
    public float strength;
    public float defence;
    public float critRate;
    public float critDamage;
    public float dodgeRate;
    public float chaseField;
    public float attackField;
    public WalkType walkType; // Will Change to AI
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Weapon")]
    public WeaponSO weapon;

    [Header("Lootings")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("References")]
    public GameObject enemyObject;
    public int angleOffset; // ?

    public enum Difficulty
    {
        Easy, Normal, Hard, Difficult, Extreme
    }

    public enum WalkType
    {
        Melee, Sniper, None
    }
}