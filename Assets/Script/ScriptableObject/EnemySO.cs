using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Name")]
    public string Name;

    [Header("Attributes Setting")]
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

    [Header("Setting")]
    public WalkType walkType;
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Weapon")]
    public WeaponSO weapon;

    [Header("Looting")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("Reference")]
    public GameObject enemyObject;
    public int angleOffset;

    public enum Difficulty
    {
        Easy, Normal, Hard, Difficult, Extreme
    }

    /*public enum ShootingType
    {
        Single, Split, AllAngle
    }

    public enum AttackType
    {
        Melee, Sniper
    }*/

    public enum WalkType
    {
        Melee, Sniper, None
    }


    /*public void Attack_Ranged(float startAngle, Vector3 startPosition)
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

        ArrowSummoned.GetComponent<Projectile>().startAngle = Quaternion.Euler(0, 0, angle + angleOffset);
        ArrowSummoned.GetComponent<Projectile>().enemyData = this;
    }*/
}