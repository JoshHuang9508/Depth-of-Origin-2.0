using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCBehaviour : MonoBehaviour, Damageable
{
    [Header("Setting")]
    public float walkSpeed;

    [Header("Dynamic Data")]
    [SerializeField] private float timeElapse;
    [SerializeField] private Vector3 currentPos, targetPos, diraction;
    [SerializeField] private bool isMoving;

    public bool movementEnabler = true;
    public float movementDisableTimer = 0;

    [Header("Object Reference")]
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;


    void Start()
    {
        currentRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        currentPos = transform.position;
        targetPos = currentPos;
    }

    void Update()
    {
        currentPos = transform.position;
        diraction = (targetPos - currentPos).normalized;

        timeElapse += Time.deltaTime;

        //update timer
        UpdateTimer();

        //actions
        Moving();
        
    }

    private void Moving()
    {
        animator.SetBool("isHit", !movementEnabler);
        spriteRenderer.flipX = Mathf.Abs(targetPos.x - currentPos.x) > 0.2 ? targetPos.x - currentPos.x < 0 : spriteRenderer.flipX;

        if (!movementEnabler) return;

        if (timeElapse >= Random.Range(3f, 5f) && !isMoving)
        {
            SetTargetPos();
            timeElapse = 0;
        }

        if (!DetectTargetPos(currentPos, targetPos))
        {
            currentRb.MovePosition(transform.position + walkSpeed * Time.deltaTime * diraction);

            if (timeElapse >= Random.Range(1f, 3f) && isMoving)
            {
                targetPos = currentPos;
                timeElapse = 0;
            }

            isMoving = true;
            animator.SetBool("ismove", true);
        }
        else
        {
            isMoving = false;
            animator.SetBool("ismove", false);
        }
    }

    private void SetTargetPos()
    {
        Vector3 newPosOffset = new(Random.Range(-3, 3), Random.Range(-3, 3), 0);

        targetPos = currentPos + newPosOffset;
    }

    private bool DetectTargetPos(Vector3 currentPos, Vector3 targetPos)
    {
        if(currentPos.x > targetPos.x - 0.2 &&
            currentPos.x < targetPos.x + 0.2 &&
            currentPos.y > targetPos.y - 0.2 &&
            currentPos.y < targetPos.y + 0.2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        currentRb.velocity = knockbackForce;

        movementDisableTimer = movementDisableTimer > knockbackTime ? movementDisableTimer : knockbackTime;
    }

    private void UpdateTimer()
    {
        movementDisableTimer = Mathf.Max(0, movementDisableTimer - Time.deltaTime);

        movementEnabler = movementDisableTimer <= 0;
    }
}
