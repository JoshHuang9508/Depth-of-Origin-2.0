using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzmodasBehavior : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("Setting")]
    public EnemySO enemy;
    public List<Vector2> positionList;

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
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject column;
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

    void Start()
    {
        currentHealth = enemy.health;
        currnetShieldHealth = enemy.shieldHealth;
        HaveShield = enemy.haveShield;
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        column.GetComponent<ShieldHolderController>().shieldBreak += RemoveShield;

        if (enemy.isBoss) gameObject.tag = "Boss";

        currentRb.bodyType = RigidbodyType2D.Static;
        noMoveTimer = 3;
        noDamageTimer = 3;
        noAttackTimer = 5;
        StartCoroutine(SetTimer(callback => {
            enable = callback;
        }, 5f));
        BuildColumns();
    }

    private void Update()
    {
        shield.SetActive(HaveShield);

        if(Health <= enemy.health * 0.5 && behaviorType == 1)
        {
            noMoveTimer = 3;
            noDamageTimer = 3;
            noAttackTimer = 5;
            behaviorType = 2;
        }

        if (Mathf.RoundToInt(noAttackTimer) == 2)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().PlayAnimator("Warning");
        }

        if (!enable) return;

        switch (behaviorType)
        {
            case 1:
                currentRb.bodyType = RigidbodyType2D.Static;
                enemy.walkType = EnemySO.WalkType.None;
                enemy.attackType = EnemySO.AttackType.Sniper;
                enemy.attackField = 100;
                enemy.chaseField = 0;
                enemy.attackSpeed = 0.4f;
                enemy.attackDamage = 1500;
                break;

            case 2:
                currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemy.walkType = EnemySO.WalkType.Melee;
                enemy.attackType = EnemySO.AttackType.Melee;
                enemy.attackField = 3f;
                enemy.chaseField = 100;
                enemy.attackSpeed = 1;
                enemy.attackDamage = 2000;
                break;
        }
    }

    public void RemoveShield()
    {
        ShieldHealth = 0;
        noAttackTimer = 65f;

        StartCoroutine(SetTimer((callback) =>{
            if (callback && behaviorType == 1)
            {
                SetShield();
                BuildColumns();
            }
        }, 60));
    }

    public void BuildColumns()
    {
        for(int i = 0; i < 6; i++)
        {
            ShieldHolderController columnSummoned = Instantiate(column, new Vector3(
                positionList[i].x, positionList[i].y, 0),
                Quaternion.identity,
                GameObject.FindWithTag("Object").transform
                ).GetComponent<ShieldHolderController>();
            columnSummoned.shieldBreak += RemoveShield;

            ShieldHolderController.Reset();
        }
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
