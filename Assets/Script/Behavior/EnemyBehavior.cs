using NUnit;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IDamageable
{
    [Header("Status")]
    [SerializeField] private float health, shieldHealth;
    public bool haveShield, canActive, canMove, canAttack, canBeDamaged, canDodge, isHit;

    [Header("Datas")]
    public EnemySO enemy;

    [Header("Weapon")]
    [SerializeField] private WeaponSO currentWeapon;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("References")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer entitySprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject itemDropper;

    //Runtime data
    private PlayerBehaviour player;
    private GameObject target;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private Vector2 diraction;
    private float facingAngle;
    private float MoveTimer
    {
        get
        {
            return MoveTimer;
        }
        set
        {
            MoveTimer = Mathf.Max(MoveTimer, value);
            canMove = MoveTimer <= 0;
        }
    }
    private float AttackTimer
    {
        get
        {
            return AttackTimer;
        }
        set
        {
            AttackTimer = Mathf.Max(AttackTimer, value);
            canAttack = AttackTimer <= 0;
        }
    }
    private float DamageTimer
    {
        get
        {
            return DamageTimer;
        }
        set
        {
            DamageTimer = Mathf.Max(DamageTimer, value);
            canBeDamaged = DamageTimer <= 0;
        }
    }
    private float DodgeTimer
    {
        get
        {
            return DodgeTimer;
        }
        set
        {
            DodgeTimer = Mathf.Max(DodgeTimer, value);
            canDodge = DodgeTimer <= 0;
        }
    }
    private float HitTimer
    {
        get
        {
            return HitTimer;
        }
        set
        {
            HitTimer = Mathf.Max(HitTimer, value);
            isHit = HitTimer <= 0;
        }
    }



    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Max(0, value);

            if (health <= 0) KillEnemy();
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

            haveShield = shieldHealth > 0;
        }
    }

    private void Start()
    {
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
        // UpdateWeapon();
        if (player != null) UpdateDirection();

        //actions
        if (canActive)
        {
            if (canMove) Moving();
            if (canAttack && Vector3.Distance(targetPos, currentPos) < enemy.attackField)
            {
                if (currentWeapon != null)
                {
                    AttackTimer = currentWeapon.attackSpeed;
                    Attacking(currentWeapon);
                }
            }
        }
    }

    // Change to A* Pathfinding
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

    private void Attacking(WeaponSO weapon)
    {
        if (weapon == null) return;

        switch (weapon.weaponType)
        {
            case WeaponType.melee:
                // MeleeAttack(weapon);
                break;
            case WeaponType.range:
                // RangedAttack(weapon);
                break;
        }
    }

    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!canBeDamaged || attackerType != AttackerType.player) return;

        if (Random.Range(0f, 100f) <= enemy.dodgeRate)
        {
            player.SetDamageText(transform.position, 0, DamageTextDisplay.DamageTextType.Dodge);

            DodgeTimer = 1f;
            DamageTimer = 0.2f;
        }
        else
        {
            float trueDamage = damage / (1 + (0.001f * enemy.defence));
            Vector2 trueKnockbackForce = knockbackForce / (1 + (0.001f * enemy.defence));
            float trueKnockbackTime = knockbackTime / (1 + (0.001f * enemy.defence));

            if (haveShield)
            {
                ShieldHealth -= trueDamage;

                // // AudioPlayer.Playsound(shieldHitSound);
            }
            else if (!haveShield)
            {
                Health -= trueDamage;

                currentRb.velocity = trueKnockbackForce;
                // AudioPlayer.Playsound(hitSound);

                MoveTimer = trueKnockbackTime;
                AttackTimer = trueKnockbackTime;
                HitTimer = trueKnockbackTime;
            }

            player.SetDamageText(transform.position, damage / (1 + (0.001f * enemy.defence)), isCrit ? DamageTextDisplay.DamageTextType.DamageCrit : DamageTextDisplay.DamageTextType.Damage);

            DamageTimer = 0.2f;
        }
    }

    private void UpdateTimer()
    {
        MoveTimer = Mathf.Max(0, MoveTimer - Time.deltaTime);
        AttackTimer = Mathf.Max(0, AttackTimer - Time.deltaTime);
        DamageTimer = Mathf.Max(0, DamageTimer - Time.deltaTime);
        DodgeTimer = Mathf.Max(0, DodgeTimer - Time.deltaTime);
        HitTimer = Mathf.Max(0, HitTimer - Time.deltaTime);
    }

    private void UpdateDirection()
    {
        currentPos = transform.position;
        targetPos = target.transform.position;
        diraction = (targetPos - currentPos).normalized;
        facingAngle = Mathf.Atan2(diraction.y, diraction.x) * Mathf.Rad2Deg;
    }

    public void KillEnemy()
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

        // AudioPlayer.Playsound(deadSound);

        Destroy(gameObject);
    }
}