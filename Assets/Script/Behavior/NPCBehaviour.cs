using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCBehaviour : MonoBehaviour, Damageable
{
    [Header("Setting")]
    public float walkSpeed;

    [Header("Reference")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    //Runtime data
    private PlayerBehaviour player;
    private float timeElapse;
    private Vector3 currentPos, targetPos, diraction;
    private bool isMove = false;
    private float noMoveTimer = 0;
    private float noDamageTimer = 0;

    public bool canActive { get; private set; }
    public bool canMove { get; private set; }
    public bool canBeDamaged { get; private set; }




    void Start()
    {
        canActive = true;

        currentPos = transform.position;
        targetPos = currentPos;
    }

    void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by NPCBehaviour.cs)");
        }

        currentPos = transform.position;

        //update timer
        UpdateTimer();

        //actions
        if (canActive)
        {
            if (canMove) Moving();
        }
    }

    private void Moving()
    {
        animator.SetBool("isHit", !canMove);
        spriteRenderer.flipX = diraction.x < 0;

        if (timeElapse >= Random.Range(3f, 15f) && !isMove)
        {
            SetDirection();
            timeElapse = 0;
            isMove = true;
            animator.SetBool("ismove", true);
        }

        currentRb.MovePosition(transform.position + walkSpeed * Time.deltaTime * diraction);

        if (timeElapse >= Random.Range(1f, 2f) && isMove)
        {
            diraction = Vector3.zero;
            timeElapse = 0;
            isMove = false;
            animator.SetBool("ismove", false);
        }
    }

    private void SetDirection()
    {
        Vector3 newPosOffset = new(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        targetPos = currentPos + newPosOffset;
        diraction = (targetPos - currentPos).normalized;
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!canBeDamaged || !canActive) return;

        float localDamage = damage;
        Vector2 localKnockbackForce = knockbackForce;
        float localKnockbackTime = knockbackTime;

        //update heath (but NPC has no health smh)

        //knockback
        currentRb.velocity = localKnockbackForce;

        //play audio (but NPC has no audio smh)

        //instantiate damege text
        player.SetDamageText(transform.position, localDamage, DamageTextDisplay.DamageTextType.PlayerHit);

        //set timer
        noMoveTimer = noMoveTimer < localKnockbackTime ? localKnockbackTime : noMoveTimer;
        noDamageTimer = 0.2f;

        //reset movement
        diraction = Vector3.zero;
        timeElapse = 0;
        isMove = false;
        animator.SetBool("ismove", false);
    }

    private void UpdateTimer()
    {
        timeElapse += Time.deltaTime;

        noMoveTimer = Mathf.Max(0, noMoveTimer - Time.deltaTime);
        noDamageTimer = Mathf.Max(0, noDamageTimer - Time.deltaTime);

        canMove = noMoveTimer <= 0;
        canBeDamaged = noDamageTimer <= 0;
    }
}
