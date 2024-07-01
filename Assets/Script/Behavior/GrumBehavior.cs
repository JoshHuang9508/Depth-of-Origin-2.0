using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrumBehavior : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deadSound;

    [Header("Setting")]
    public EnemySO enemy;
    [SerializeField] private SlimeSplitStage slimeSplitStage;
    [SerializeField]
    private enum SlimeSplitStage
    {
        Big, Mid, Small
    }

    [Header("Dynamic Data")]
    [SerializeField] private bool enable = true;

    [SerializeField] private GameObject target;
    [SerializeField] private float currentHealth, currnetShieldHealth;

    [SerializeField] private float noMoveTimer = 0;
    [SerializeField] private float noAttackTimer = 0;
    [SerializeField] private float noDamageTimer = 0;
    [SerializeField] private float noDodgeTimer = 0;
    [SerializeField] private float isHitTimer = 0;

    [Header("Object Reference")]
    [SerializeField] private GameObject poison;
    [SerializeField] private EnemySO smallerSlime;
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
    


    private void Update()
    {
        switch (slimeSplitStage)
        {
            case SlimeSplitStage.Big:

                float distant = Vector3.Distance(CurrentPos, TargetPos);

                if (distant > 6f)
                {
                    currentRb.bodyType = RigidbodyType2D.Dynamic;
                    enemy.attackType = EnemySO.AttackType.Sniper;
                    enemy.attackField = 100;
                    enemy.chaseField = 100;
                    enemy.knockbackForce = 30f;

                    poison.SetActive(false);
                }
                else if (distant <= 6f)
                {
                    currentRb.bodyType = RigidbodyType2D.Dynamic;
                    enemy.attackType = EnemySO.AttackType.Melee;
                    enemy.attackField = 3f;
                    enemy.chaseField = 100;
                    enemy.knockbackForce = 100f;

                    poison.SetActive(true);
                }
                break;

            case SlimeSplitStage.Mid:

                currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemy.attackType = EnemySO.AttackType.Melee;
                enemy.attackField = 2;
                enemy.chaseField = 100;
                enemy.knockbackForce = 30f;

                break;

            case SlimeSplitStage.Small:

                currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemy.attackType = EnemySO.AttackType.Melee;
                enemy.attackField = 1.5f;
                enemy.chaseField = 100;
                enemy.knockbackForce = 15f;

                break;
        }
    }

    private void OnDestroy()
    {
        if(slimeSplitStage != SlimeSplitStage.Small)
        {
            for (int i = 0; i < 2; i++)
            {
                var bossSummoned = Instantiate(
                    smallerSlime.EnemyObject,
                    transform.position,
                    Quaternion.identity,
                    GameObject.FindWithTag("Entity").transform);
                bossSummoned.GetComponent<EnemyBehavior>().enemy = smallerSlime;
            }
        }
    }
}
