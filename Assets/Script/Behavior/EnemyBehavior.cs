using NUnit;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, Damageable
{
    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float shieldHealth;

    [Header("Setting")]
    public EnemySO enemy;

    [Header("Weapon")]
    [SerializeField] private WeaponSO currentWeapon;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("Reference")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer entitySprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject itemDropper;

    //Runtime data
    private PlayerBehaviour player;
    private GameObject target;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private Vector2 diraction;
    private float facingAngle;
    private float noMoveTimer = 0;
    private float noAttackTimer = 0;
    private float noDamageTimer = 0;
    private float noDodgeTimer = 0;
    private float isHitTimer = 0;

    public bool haveShield { get; private set; }
    public bool canActive { get; private set; }
    public bool canMove { get; private set; }
    public bool canAttack { get; private set; }
    public bool canBeDamaged { get; private set; }
    public bool canDodge { get; private set; }
    public bool isHit { get; private set; }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Max(0, value);

            if (health <= 0)
            {
                //drop items
                ItemDropper ItemDropper = Instantiate(
                    itemDropper,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    Quaternion.identity,
                    GameObject.FindWithTag("Item").transform
                    ).GetComponent<ItemDropper>();
                ItemDropper.DropItems(enemy.lootings);
                ItemDropper.DropCoins(enemy.coins);
                ItemDropper.DropWrackages(enemy.wreckage);

                audioPlayer.PlayOneShot(deadSound);

                Destroy(gameObject);
            }
        }
    }

    public float ShieldHealth
    {
        get
        {
            return shieldHealth;
        }
        set
        {
            shieldHealth = Mathf.Max(0, value);

            if(shieldHealth <= 0) haveShield = false;

            if (shieldHealth > 0) haveShield = true;
        }
    }

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        health = enemy.maxHealth;
        shieldHealth = enemy.maxShieldHealth;
        haveShield = enemy.haveShield;
        currentWeapon = enemy.weapon;
        canActive = true;

        if (enemy.isBoss) gameObject.tag = "Boss";
    }

    private void Update()
    {
        try 
        { 
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
            target = player.gameObject;
        } 
        catch
        {
            Debug.LogWarning("Can't find player (sent by enemyBehaviour.cs)");
        }

        animator.enabled = !isHit;

        //update timer
        UpdateTimer();
        UpdateWeapon();
        if(player != null) UpdateDirection();

        //actions
        if (canActive)
        {
            if (canMove) Moving();
            if (canAttack && Vector3.Distance(targetPos, currentPos) < enemy.attackField)
            {
                if (currentWeapon != null)
                {
                    noAttackTimer += currentWeapon.attackCooldown;
                    Attacking();
                }
            }
        }
    }

    private void Moving()
    {
        entitySprite.flipX = (currentPos.x - targetPos.x) > 0.2;

        switch (enemy.walkType)
        {
            case EnemySO.WalkType.Melee:

                if (Vector3.Distance(targetPos, currentPos) < enemy.chaseField && Vector3.Distance(targetPos, currentPos) > enemy.attackField)
                {
                    currentRb.MovePosition(currentPos + enemy.walkSpeed * Time.deltaTime * diraction);

                    animator.SetBool("ismove", true);
                    animator.SetBool("ischase", true);
                }
                else if (Vector3.Distance(targetPos, currentPos) > enemy.chaseField)
                {
                    currentRb.velocity = Vector2.zero;

                    animator.SetBool("ismove", false);
                    animator.SetBool("ischase", false);
                }
                else if (Vector3.Distance(targetPos, currentPos) < enemy.chaseField && Vector3.Distance(targetPos, currentPos) < enemy.attackField)
                {
                    currentRb.velocity = Vector2.zero;

                    animator.SetBool("ismove", false);
                    animator.SetBool("ischase", false);
                }
                break;

            case EnemySO.WalkType.Sniper:

                if (Vector3.Distance(targetPos, currentPos) < enemy.chaseField)
                {
                    currentRb.MovePosition(currentPos - enemy.walkSpeed * Time.deltaTime * diraction);

                    animator.SetBool("ismove", true);
                    animator.SetBool("ischase", true);
                }
                else if (Vector3.Distance(targetPos, currentPos) > enemy.chaseField)
                {
                    currentRb.velocity = Vector2.zero;

                    animator.SetBool("ismove", false);
                    animator.SetBool("ischase", false);
                }
                break;

            case EnemySO.WalkType.None:

                break;
        }
    }

    private void Attacking()
    {
        WeaponSO weapon = currentWeapon;

        if (weapon == null) return;

        for (var i = weaponSprite.gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(weaponSprite.gameObject.transform.GetChild(i).gameObject);
        }

        if (weapon is MeleeWeaponSO)
        {
            MeleeWeaponSO meleeWeapon = weapon as MeleeWeaponSO;

            var sword = Instantiate(
                meleeWeapon.weaponObject,
                weaponSprite.gameObject.transform.position,
                Quaternion.identity,
                weaponSprite.gameObject.transform
                ).GetComponent<MeleeWeapon>();

            sword.target = Target.player;
            sword.weapon = meleeWeapon;
            sword.strength = enemy.strength;
            sword.critRate = enemy.critRate;
            sword.critDamage = enemy.critDamage;
            sword.isflip = diraction.x < 0;

            weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle - 90);
        }
        else if (weapon is RangedWeaponSO)
        {
            RangedWeaponSO rangedWeapon = weapon as RangedWeaponSO;

            switch (rangedWeapon.shootingType)
            {
                case RangedWeaponSO.ShootingType.Single:

                    var projectile = Instantiate(
                        rangedWeapon.projectileObject,
                        weaponSprite.gameObject.transform.position,
                        Quaternion.Euler(0, 0, facingAngle - 90),
                        GameObject.FindWithTag("Item").transform
                        ).GetComponent<RangedWeapon>();

                    projectile.target = Target.player;
                    projectile.weapon = rangedWeapon;
                    projectile.strength = enemy.strength;
                    projectile.critRate = enemy.critRate;
                    projectile.critDamage = enemy.critDamage;
                    projectile.startAngle = Quaternion.Euler(0, 0, facingAngle);
                    break;

                case RangedWeaponSO.ShootingType.Split:

                    for (int i = -60 + (120 / (rangedWeapon.splitAmount + 1)); i < 60; i += 120 / (rangedWeapon.splitAmount + 1))
                    {
                        var projectile_split = Instantiate(
                            rangedWeapon.projectileObject,
                            weaponSprite.gameObject.transform.position,
                            Quaternion.Euler(0, 0, facingAngle + i - 90),
                            GameObject.FindWithTag("Item").transform
                            ).GetComponent<RangedWeapon>(); ;

                        projectile_split.target = Target.player;
                        projectile_split.weapon = rangedWeapon;
                        projectile_split.strength = enemy.strength;
                        projectile_split.critRate = enemy.critRate;
                        projectile_split.critDamage = enemy.critDamage;
                        projectile_split.startAngle = Quaternion.Euler(0, 0, facingAngle + i);
                    }
                    break;
            }
        }
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!canBeDamaged) return;

        bool isDodge = Random.Range(0f, 100f) <= enemy.dodge;

        if (canDodge && isDodge)
        {
            //instantiate damage text
            player.SetDamageText(transform.position, 0, DamageTextDisplay.DamageTextType.Dodge);

            //set timer
            noDodgeTimer = 1f;
            noDamageTimer = 0.2f;
        }
        else
        {
            float localDamage = damage / (1 + (0.001f * enemy.defence));
            Vector2 localKnockbackForce = knockbackForce / (1 + (0.001f * enemy.defence));
            float localKnockbackTime = knockbackTime / (1 + (0.001f * enemy.defence));

            if (haveShield)
            {
                //update shield health
                ShieldHealth -= localDamage;
            }
            else if (!haveShield)
            {
                //update heath
                Health -= localDamage;

                //knockback
                currentRb.velocity = localKnockbackForce;

                //play audio
                audioPlayer.PlayOneShot(hitSound);

                //set timer
                noMoveTimer = noMoveTimer < localKnockbackTime ? localKnockbackTime : noMoveTimer;
                noAttackTimer = noAttackTimer < localKnockbackTime ? localKnockbackTime : noAttackTimer;
                isHitTimer = isHitTimer < localKnockbackTime ? localKnockbackTime : isHitTimer;
            }

            //instantiate damage text
            player.SetDamageText(transform.position, damage / (1 + (0.001f * enemy.defence)), isCrit ? DamageTextDisplay.DamageTextType.DamageCrit : DamageTextDisplay.DamageTextType.Damage);

            //set timer
            noDamageTimer = 0.2f;
        }
    }

    private bool UpdateTimer()
    {
        noMoveTimer = Mathf.Max(0, noMoveTimer - Time.deltaTime);
        noAttackTimer = Mathf.Max(0, noAttackTimer - Time.deltaTime);
        noDamageTimer = Mathf.Max(0, noDamageTimer - Time.deltaTime);
        noDodgeTimer = Mathf.Max(0, noDodgeTimer - Time.deltaTime);
        isHitTimer = Mathf.Max(0, isHitTimer - Time.deltaTime);

        canMove = noMoveTimer <= 0;
        canAttack = noAttackTimer <= 0;
        canBeDamaged = noDamageTimer <= 0;
        canDodge = noDodgeTimer <= 0;
        isHit = !(isHitTimer <= 0);

        return true;
    }

    private void UpdateDirection()
    {
        currentPos = transform.position;
        targetPos = target.transform.position;
        diraction = (targetPos - currentPos).normalized;
        facingAngle = Mathf.Atan2(diraction.y, diraction.x) * Mathf.Rad2Deg;
    }

    private void UpdateWeapon()
    {
        //update current weapon
        if (currentWeapon == null)
        {
            weaponSprite.sprite = null;
            weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle);
        }
        else
        {
            if (currentWeapon is MeleeWeaponSO) weaponSprite.sprite = null;
            else if (currentWeapon is RangedWeaponSO)
            {
                weaponSprite.sprite = currentWeapon.Image;
                weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle);
            }
        }
    }
}