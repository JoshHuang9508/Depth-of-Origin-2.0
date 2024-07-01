using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "new enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Basic Data")]
    public string Name;

    [Header("Setting")]
    public float health;
    public bool haveShield;
    public float shieldHealth;
    public WalkType walkType;
    public float moveSpeed;
    public float defence;
    public bool isBoss;
    public AttackType attackType;
    public float attackSpeed;
    public float attackDamage;
    public float chaseField;
    public float attackField;
    public float knockbackForce;
    public float knockbackTime;
    public ShootingType shootingType;
    public float projectileFlySpeed;
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Looting")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("Object Reference")]
    public GameObject EnemyObject;
    public GameObject projectile;
    public int angleOffset;

    public enum Difficulty
    {
        Easy, Normal, Hard, Difficult, Extreme
    }

    public enum ShootingType
    {
        Single, Split, AllAngle
    }

    public enum AttackType
    {
        Melee, Sniper
    }

    public enum WalkType
    {
        Melee, Sniper, None
    }


    public void Attack_Ranged(float startAngle, Vector3 startPosition)
    {
        switch (shootingType)
        {
            case ShootingType.Single:
                SummonArrow(startPosition, startAngle);
                break;
                
            case ShootingType.Split:
                for (int i = -60; i <= 60; i += 30)
                {
                    SummonArrow(startPosition, startAngle + i);
                }
                break;

            case ShootingType.AllAngle:
                for (int i = -180; i < 180; i += 18)
                {
                    SummonArrow(startPosition, startAngle + i);
                }
                break;
        }
    }

    private void SummonArrow(Vector3 position, float angle)
    {
        var ArrowSummoned = Instantiate(
                        projectile,
                        position,
                        Quaternion.Euler(0, 0, angle - 90),
                        GameObject.FindWithTag("Item").transform);

        ArrowSummoned.GetComponent<ProjectileMovement_Enemy>().startAngle = Quaternion.Euler(0, 0, angle + angleOffset);
        ArrowSummoned.GetComponent<ProjectileMovement_Enemy>().enemyData = this;
    }
}