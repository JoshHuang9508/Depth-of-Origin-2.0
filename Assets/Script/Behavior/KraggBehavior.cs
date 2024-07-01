using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KraggBehavior : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("Setting")]
    public EnemySO enemy;
    [SerializeField] private float disableTimer = 10;

    [Header("Dynamic Data")]
    [SerializeField] private bool enable = true;

    [SerializeField] private GameObject target;
    [SerializeField] private float currentHealth, currnetShieldHealth;
    [SerializeField] private int behaviorType = 1;

    [SerializeField] private float noMoveTimer = 0;
    [SerializeField] private float noAttackTimer = 0;
    [SerializeField] private float noDamageTimer = 0;
    [SerializeField] private float noDodgeTimer = 0;
    [SerializeField] private float isHitTimer = 0;

    [Header("Object Reference")]
    [SerializeField] private GameObject firstProjectile;
    [SerializeField] private GameObject seconedProjectile;
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject itemDropper;

    public float Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;

            if (currentHealth <= 0)
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
            return currnetShieldHealth;
        }
        set
        {
            currnetShieldHealth = value;

            if (currnetShieldHealth <= 0)
            {
                HaveShield = false;
            }
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


    private void Start()
    {
        //enemyBehavior.OnAttack += Attacking;

        currentRb.bodyType = RigidbodyType2D.Static;
        noMoveTimer = 3;
        noDamageTimer = 3;
        noAttackTimer = 13;

        StartCoroutine(SetTimer(callback => {
            enable = callback;
        }, 5f));
    }

    private void Update()
    {
        if (Health <= enemy.health * 0.5 && behaviorType == 1)
        {
            noMoveTimer = 3;
            noDamageTimer = 3;
            noAttackTimer = 5;
            behaviorType = 2;
        }

        if (!enable) return;

        if (Mathf.RoundToInt(noAttackTimer) == 2)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().PlayAnimator("Warning");
        }

        switch (behaviorType)
        {
            case 1:
                
                currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemy.shootingType = EnemySO.ShootingType.AllAngle;
                enemy.moveSpeed = 2;
                enemy.attackSpeed = 10;
                enemy.projectile = firstProjectile;

                if (HaveShield)
                {
                    disableTimer = 10;
                }
                else if (!HaveShield)
                {
                    noMoveTimer = 3;
                    noAttackTimer = 13;

                    disableTimer -= Time.deltaTime;

                    ShieldHealth = enemy.shieldHealth * (1 - Mathf.Max(disableTimer / 10, 0));

                    if (disableTimer <= 0) SetShield();
                }
                break;

            case 2:

                currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemy.shootingType = EnemySO.ShootingType.Single;
                enemy.moveSpeed = 6;
                enemy.attackSpeed = 0.6f;
                enemy.projectile = seconedProjectile;
                ShieldHealth = 0;

                break;
        }
    }

    private void Attacking()
    {
        ShieldHealth -= enemy.shieldHealth * 0.25f;
    }

    private IEnumerator SetTimer(System.Action<bool> callback, float time)
    {
        callback(false);
        yield return new WaitForSeconds(time);
        callback(true);
    }

    public void SetShield(float _shieldHealth = 0)
    {
        ShieldHealth = _shieldHealth == 0 ? enemy.shieldHealth : _shieldHealth;

        HaveShield = true;
    }
}
