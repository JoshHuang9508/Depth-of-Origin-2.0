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
    [SerializeField] private bool isMove = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float noMoveTimer = 0;

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

        //update timer
        UpdateTimer();

        //actions
        if (canMove) Moving();
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

    /*private bool IsOnTargetPos()
    {
        return (
        currentPos.x > (targetPos.x - 0.2) &&
        currentPos.x < (targetPos.x + 0.2) &&
        currentPos.y > (targetPos.y - 0.2) &&
        currentPos.y < (targetPos.y + 0.2) );
    }*/

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        currentRb.velocity = knockbackForce;

        noMoveTimer = noMoveTimer > knockbackTime ? noMoveTimer : knockbackTime;

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

        canMove = noMoveTimer <= 0;
    }
}
