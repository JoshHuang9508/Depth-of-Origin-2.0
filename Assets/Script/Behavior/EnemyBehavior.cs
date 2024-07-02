using NUnit;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, Damageable
{
    [Header("Stats")]
    [SerializeField] private float health, shieldHealth;

    [Header("Setting")]
    public EnemySO enemy;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    //Runtime data
    private bool canActive = true;
    private PlayerBehaviour player;
    private GameObject target;

    private float noMoveTimer = 0;
    private float noAttackTimer = 0;
    private float noDamageTimer = 0;
    private float noDodgeTimer = 0;
    private float isHitTimer = 0;

    [Header("Reference")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject itemDropper;

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

            if(shieldHealth <= 0) HaveShield = false;

            if (shieldHealth > 0) HaveShield = true;
        }
    }
    public bool HaveShield { get; private set; }
    public bool CanMove { get; private set; }
    public bool CanAttack { get; private set; }
    public bool CanBeDamaged { get; private set; }
    public bool CanDodge { get; private set; }
    public bool IsHit { get; private set; }
    public Vector2 CurrentPos { get; private set; }
    public Vector2 TargetPos { get; private set; }
    public Vector2 Diraction { get; private set; }

    public delegate void EnemyAttack();
    public event EnemyAttack OnAttack;



    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        health = enemy.health;
        shieldHealth = enemy.shieldHealth;
        HaveShield = enemy.haveShield;

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

        if (!canActive) return;

        if (player != null)
        {
            CurrentPos = transform.position;
            TargetPos = target.transform.position;
            Diraction = (TargetPos - CurrentPos).normalized;
        }

        animator.enabled = !IsHit;

        //actions
        if (CanMove) Moving();
        if(CanAttack) Attacking();

        //update timer
        UpdateTimer();
    }

    private void Moving()
    {
        spriteRenderer.flipX = (CurrentPos.x - TargetPos.x) > 0.2;

        switch (enemy.walkType)
        {
            case EnemySO.WalkType.Melee:

                if (Vector3.Distance(TargetPos, CurrentPos) < enemy.chaseField)
                {
                    currentRb.MovePosition(CurrentPos + enemy.moveSpeed * Time.deltaTime * Diraction);

                    animator.SetBool("ismove", true);
                    animator.SetBool("ischase", true);
                }
                else if (Vector3.Distance(TargetPos, CurrentPos) > enemy.chaseField)
                {
                    animator.SetBool("ismove", false);
                    animator.SetBool("ischase", false);
                }
                break;

            case EnemySO.WalkType.Sniper:

                if (Vector3.Distance(TargetPos, CurrentPos) < enemy.chaseField)
                {
                    currentRb.MovePosition(CurrentPos - enemy.moveSpeed * Time.deltaTime * Diraction);

                    animator.SetBool("ismove", true);
                    animator.SetBool("ischase", true);
                }
                else if (Vector3.Distance(TargetPos, CurrentPos) > enemy.chaseField)
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
        switch (enemy.attackType)
        {
            case EnemySO.AttackType.Melee:

                if (Vector3.Distance(TargetPos, CurrentPos) < enemy.attackField)
                {
                    Damageable damageableObject = target.GetComponent<Damageable>();

                    if(damageableObject != null)
                    {
                        damageableObject.OnHit(enemy.attackDamage, false, Diraction * enemy.knockbackForce, enemy.knockbackTime);

                        noAttackTimer += enemy.attackSpeed;
                        noMoveTimer += enemy.attackSpeed;

                        if(OnAttack != null) OnAttack.Invoke();
                    }
                }
                break;

            case EnemySO.AttackType.Sniper:

                if (Vector3.Distance(TargetPos, CurrentPos) < enemy.attackField)
                {
                    enemy.Attack_Ranged(Mathf.Atan2(Diraction.y, Diraction.x) * Mathf.Rad2Deg, transform.position + new Vector3(0, 0.5f, 0));

                    noAttackTimer += enemy.attackSpeed;

                    if (OnAttack != null) OnAttack.Invoke();
                }
                break;
        }
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!CanBeDamaged || !canActive) return;

        float localDamage = damage / (1 + (0.001f * enemy.defence));
        Vector2 localKnockbackForce = knockbackForce / (1 + (0.001f * enemy.defence));
        float localKnockbackTime = knockbackTime / (1 + (0.001f * enemy.defence));

        if (HaveShield)
        {
            //update shield health
            ShieldHealth -= localDamage;
        }
        else if (!HaveShield)
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
        player.SetDamageText(transform.position, damage / (1 + (0.001f * enemy.defence)), isCrit ? "DamageCrit" : "Damage");

        //set timer
        noDamageTimer += 0.2f;
    }

    private bool UpdateTimer()
    {
        noMoveTimer = Mathf.Max(0, noMoveTimer - Time.deltaTime);
        noAttackTimer = Mathf.Max(0, noAttackTimer - Time.deltaTime);
        noDamageTimer = Mathf.Max(0, noDamageTimer - Time.deltaTime);
        noDodgeTimer = Mathf.Max(0, noDodgeTimer - Time.deltaTime);
        isHitTimer = Mathf.Max(0, isHitTimer - Time.deltaTime);

        CanMove = noMoveTimer <= 0;
        CanAttack = noAttackTimer <= 0;
        CanBeDamaged = noDamageTimer <= 0;
        CanDodge = noDodgeTimer <= 0;
        IsHit = !(isHitTimer <= 0);

        return true;
    }
}