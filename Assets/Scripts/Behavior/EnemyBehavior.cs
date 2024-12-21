using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IDamageable
{
    [Header("Datas")]
    public EnemySO enemy;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound, deadSound, dodgeSound;

    [Header("References")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer entitySprite;
    [SerializeField] private Animator animator;

    // Status
    public float Health { get; private set; }
    public float ShieldHealth { get; private set; }

    // Effection
    public List<Effection> effectionList = new();
    private float maxHealth_e, walkSpeed_e, strength_e, defence_e, critRate_e, critDamage_e;

    // Weapon
    private WeaponSO weapon;

    // Flags
    public bool isActive, haveShield;

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
        Health = enemy.maxHealth;
        ShieldHealth = enemy.maxShieldHealth;
        haveShield = enemy.haveShield;
        weapon = enemy.weapon;
        isActive = true;

        if (enemy.isBoss) gameObject.tag = "Boss";
    }

    private void Update()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        target = player.gameObject;

        animator.enabled = IsTimerEnd(TimerType.Hit);
        entitySprite.flipX = (targetPos - currentPos).normalized.x < 0;

        UpdateTimer();
        UpdatePos();

        if (!isActive) return;
        if (IsTimerEnd(TimerType.Move) && Vector2.Distance(targetPos, currentPos) <= enemy.chaseField) Move();
        if (IsTimerEnd(TimerType.Attack) && Vector3.Distance(targetPos, currentPos) < enemy.attackField && weapon != null) Attack(weapon);
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
                    path = Pathfinder.FindPath(currentPos, targetPos, enemy.chaseField, 0.4f);
                    if (path != null) pathFollower = StartCoroutine(FollowPath());
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
        Health += Mathf.Min(value, enemy.maxHealth - Health);
    }
    private void Damage(float value)
    {
        Health -= Mathf.Min(value, 0);
        DamageTextGenerator.SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        AudioPlayer.PlaySound(hitSound);
        if (Health <= 0) Kill();
    }
    private void ShieldDamage(float value)
    {
        ShieldHealth -= Mathf.Min(value, 0);
        DamageTextGenerator.SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        AudioPlayer.PlaySound(hitSound);
        if (ShieldHealth <= 0) haveShield = false;
    }
    private void Dodge(float value)
    {
        DamageTextGenerator.SetDamageText(transform.position, 0, DamageTextDisplay.DamageTextType.Dodge);
        AudioPlayer.PlaySound(dodgeSound);
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

        SetTimer(TimerType.Attack, weapon.attackSpeed);
    }
    private void Kill()
    {
        ItemDropper.Drop(transform.position, enemy.lootings);
        ItemDropper.Drop(transform.position, enemy.coins);
        AudioPlayer.PlaySound(deadSound);
        Destroy(gameObject);
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

        currentPos = transform.position;
        targetPos = target.transform.position;
    }

    ////////////////
    // Properties //
    ////////////////

    public void TakeDamage(AttackerType attacker, float damage, bool isCrit, Vector2 kbForce, float kbTime)
    {
        if (!IsTimerEnd(TimerType.Damage) || !isActive || attacker != AttackerType.player) return;

        float trueDamage = damage / (1 + (0.001f * enemy.defence));
        Vector2 trueKbForce = kbForce / (1 + (0.001f * enemy.defence));
        float trueKbTime = kbTime / (1 + (0.001f * enemy.defence));

        if (Random.Range(0f, 100f) <= enemy.dodgeRate)
        {
            Dodge(trueDamage);

            SetTimer(TimerType.Dodge, 1f);
        }
        else
        {
            if (haveShield)
            {
                ShieldDamage(trueDamage);
            }
            else if (!haveShield)
            {
                Damage(trueDamage);
                currentRb.velocity = trueKbForce;

                SetTimer(TimerType.Hit, trueKbTime);
                SetTimer(TimerType.Attack, trueKbTime);
                SetTimer(TimerType.Move, trueKbTime);
            }
        }
        SetTimer(TimerType.Damage, 0.2f);
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