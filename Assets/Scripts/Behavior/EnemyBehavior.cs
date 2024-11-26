using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IDamageable
{
    [Header("Datas")]
    public EnemySO enemy;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("References")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer entitySprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject itemDropper;

    // Status
    public float health { get; private set; }
    public float shieldHealth { get; private set; }

    // Effection
    public List<Effection> effectionList = new();
    private float maxHealth_e, walkSpeed_e, strength_e, defence_e, critRate_e, critDamage_e;

    // Weapon
    private WeaponSO weapon;

    // Flags
    public bool isActive, haveShield, isPlayerMove;

    // Timer
    public enum TimerType { Move, Attack, Damage, Dodge, Hit, Path }
    private Dictionary<TimerType, float> timerList = new()
    {
        { TimerType.Move, 0 },
        { TimerType.Attack, 0 },
        { TimerType.Damage, 0 },
        { TimerType.Dodge, 0 },
        { TimerType.Hit, 0 },
        { TimerType.Path, 0 }
    };

    // Runtime data
    private PlayerBehaviour player;
    private GameObject target;

    // Path | will change to A* Pathfinding
    private Vector2 currentPos, targetPos;
    private List<Vector2> path;
    private Coroutine pathFollower = null;

    private void Start()
    {
        health = enemy.maxHealth;
        shieldHealth = enemy.maxShieldHealth;
        haveShield = enemy.haveShield;
        weapon = enemy.weapon;
        isActive = true;

        if (enemy.isBoss) gameObject.tag = "Boss";
    }

    private void Update()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        target = player?.gameObject ?? null;

        animator.enabled = IsTimerEnd(TimerType.Hit);
        entitySprite.flipX = (targetPos - currentPos).normalized.x > 0;

        UpdateTimer();
        // UpdateWeapon();
        UpdatePos();

        //actions
        if (isActive)
        {
            if (IsTimerEnd(TimerType.Move) && Vector2.Distance(targetPos, currentPos) <= enemy.chaseField) Move();
            if (IsTimerEnd(TimerType.Attack) && Vector3.Distance(targetPos, currentPos) < enemy.attackField && weapon != null)
            {
                SetTimer(TimerType.Attack, weapon.attackSpeed);
                Attack(weapon);
            }
        }
    }

    ///////////////
    // Abilities //
    ///////////////

    // Change to A* Pathfinding
    private void Move()
    {
        switch (enemy.walkType)
        {
            case EnemySO.WalkType.Melee:
                if (IsTimerEnd(TimerType.Path))
                {
                    if (pathFollower != null) StopCoroutine(pathFollower);
                    SetTimer(TimerType.Path, 2f);
                    path = PathfindingManager.FindPath(currentPos, targetPos, enemy.chaseField, 0.4f);
                    pathFollower = StartCoroutine(FollowPath());
                }
                // animator.SetBool("ismove", true);
                // animator.SetBool("ischase", true);
                break;
            case EnemySO.WalkType.Sniper:


            case EnemySO.WalkType.None:
                break;
        }
    }
    private void Heal(float value)
    {
        health += Mathf.Min(value, enemy.maxHealth - health);
    }
    private void Damage(float value)
    {
        health -= value;
        // SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        // AudioPlayer.Playsound(hitSound);
    }
    private void ShieldDamage(float value)
    {
        shieldHealth -= value;
        // SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        // AudioPlayer.Playsound(hitSound);
    }
    private void Dodge(float value)
    {
        // SetDamageText(transform.position, 0, DamageTextDisplay.DamageTextType.Dodge);
        // AudioPlayer.Playsound(dodgeSound);
    }
    private void Attack(WeaponSO weapon)
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
        if (!IsTimerEnd(TimerType.Damage) || attackerType != AttackerType.player) return;

        if (Random.Range(0f, 100f) <= enemy.dodgeRate)
        {
            Dodge(damage);

            SetTimer(TimerType.Dodge, 1f);
            SetTimer(TimerType.Damage, 0.2f);
        }
        else
        {
            float trueDamage = damage / (1 + (0.001f * enemy.defence));
            Vector2 trueKnockbackForce = knockbackForce / (1 + (0.001f * enemy.defence));
            float trueKnockbackTime = knockbackTime / (1 + (0.001f * enemy.defence));

            if (haveShield)
            {
                ShieldDamage(trueDamage);
            }
            else if (!haveShield)
            {
                Damage(trueDamage);
                currentRb.velocity = trueKnockbackForce;

                SetTimer(TimerType.Hit, trueKnockbackTime);
                SetTimer(TimerType.Attack, trueKnockbackTime);
                SetTimer(TimerType.Move, trueKnockbackTime);
            }

            SetTimer(TimerType.Damage, 0.2f);
        }
    }
    private IEnumerator FollowPath()
    {
        int currentIndex = 0;

        while (currentIndex < path.Count - 1)
        {
            Vector2 nextPos = path != null && path.Count > 0 ? path[currentIndex + 1] : currentPos;
            Vector2 direction = (nextPos - currentPos).normalized;

            while (Vector2.Distance(currentPos, nextPos) > 0.1f)
            {
                currentRb.MovePosition(currentPos + enemy.walkSpeed * Time.deltaTime * direction);
                yield return null;
            }

            currentIndex++;
        }
    }

    ////////////
    // Update //
    ////////////

    private void UpdateTimer()
    {
        List<TimerType> keys = new(timerList.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            TimerType timerType = keys[i];
            timerList[timerType] -= Time.deltaTime;
        }
    }
    private void UpdatePos()
    {
        if (target == null) return;

        if (targetPos != (Vector2)target.transform.position) isPlayerMove = true;
        currentPos = transform.position;
        targetPos = target.transform.position;
    }

    ////////////////
    // Properties //
    ////////////////

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
    public void SetTimer(TimerType timerType, float value)
    {
        timerList[timerType] = value;
    }
    public bool IsTimerEnd(TimerType timerType)
    {
        return timerList[timerType] <= 0;
    }
}