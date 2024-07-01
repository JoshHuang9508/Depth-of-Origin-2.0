using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonWeapon : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private WeaponSO weapon;
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Dynamic Data")]
    [SerializeField] Vector2 mousePos;
    [SerializeField] Vector2 currentPos;
    [SerializeField] Vector2 Diraction;

    [Header("Stats")]
    public bool isflip;
    public float startAngle;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos = transform.position;
        Diraction = (mousePos - currentPos).normalized;
        startAngle = Mathf.Atan2(Diraction.y, Diraction.x) * Mathf.Rad2Deg;

        if (Diraction.x < 0)
        {
            isflip = true;
        }
        else if (Diraction.x >= 0)
        {
            isflip = false;
        }


        weapon = player.UpdateCurrentWeapon();

        if (weapon == null)
        {
            spriteRenderer.sprite = null;
            weapon = null;
        }
        else
        {
            if (weapon is MeleeWeaponSO) spriteRenderer.sprite = null;
            else if (weapon is RangedWeaponSO)
            {
                spriteRenderer.sprite = weapon.Image;
                transform.rotation = Quaternion.Euler(0, 0, startAngle);
            }
        }
    }

    public void Summon()
    {
        weapon = player.UpdateCurrentWeapon();

        if (weapon == null) return;

        for (var i = this.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }

        if (weapon is MeleeWeaponSO)
        {
            MeleeWeaponSO meleeWeapon = weapon as MeleeWeaponSO;

            var meleeWeaponSummoned = Instantiate(
                meleeWeapon.weaponObject,
                transform.position,
                Quaternion.identity,
                this.transform
                ).GetComponent<WeaponMovementMelee>();

            meleeWeaponSummoned.weaponData = meleeWeapon;
            meleeWeaponSummoned.playerData = player;
            meleeWeaponSummoned.isflip = isflip;

            transform.rotation = Quaternion.Euler(0, 0, startAngle - 90);
        }
        else if (weapon is RangedWeaponSO)
        {
            RangedWeaponSO rangedWeapon = weapon as RangedWeaponSO;

            switch (rangedWeapon.shootingType)
            {
                case RangedWeaponSO.ShootingType.Single:

                    var projectileSummoned = Instantiate(
                        rangedWeapon.projectileObject,
                        transform.position,
                        Quaternion.Euler(0, 0, startAngle - 90),
                        GameObject.FindWithTag("Item").transform
                        ).GetComponent<WeaponMovementRanged>();

                    projectileSummoned.weaponData = rangedWeapon;
                    projectileSummoned.playerData = player;
                    projectileSummoned.startAngle = Quaternion.Euler(0, 0, startAngle);
                    break;

                case RangedWeaponSO.ShootingType.Split:

                    for (int i = -60 + (120 / (rangedWeapon.splitAmount + 1)); i < 60; i += 120 / (rangedWeapon.splitAmount + 1))
                    {
                        var projectileSummoned_split = Instantiate(
                            rangedWeapon.projectileObject,
                            transform.position,
                            Quaternion.Euler(0, 0, startAngle + i - 90),
                            GameObject.FindWithTag("Item").transform
                            ).GetComponent<WeaponMovementRanged>(); ;

                        projectileSummoned_split.weaponData = rangedWeapon;
                        projectileSummoned_split.playerData = player;
                        projectileSummoned_split.startAngle = Quaternion.Euler(0, 0, startAngle + i);
                    }
                    break;
            }
        }
    }
}
