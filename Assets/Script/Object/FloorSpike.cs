using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorSpike : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private bool isActive = false;

    [Header("Attributes")]
    [SerializeField] private float damage;
    [SerializeField] private float inactiveTime;
    [SerializeField] private float activeTime;

    [Header("Reference")]
    [SerializeField] private Animator animator;

    //Runtime data
    private float timer;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateTimer();

        Active();
    }

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActive) return;

        if (collision is IDamageable)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable?.Damage(AttackerType.enemy, damage, false, Vector2.zero, 0);
        }
    }

    // private bool DetectPlayer()
    // {
    //     List<Collider2D> colliderResult = new();
    //     Physics2D.OverlapCollider(GetComponent<Collider2D>(), new(), colliderResult);

    //     bool isPlayerInRange = false;
    //     for (int i = 0; i < colliderResult.Count; i++)
    //     {
    //         if (colliderResult[i] != null && colliderResult[i].CompareTag("Player")) isPlayerInRange = true;
    //     }

    //     return isPlayerInRange;
    // }

    private void Active()
    {
        animator.SetBool("isActive", isActive);

        if (timer >= inactiveTime && !isActive)
        {
            isActive = true;
            timer = 0;
        }
        else if (timer >= activeTime && isActive)
        {
            isActive = false;
            timer = 0;
        }
    }
}
