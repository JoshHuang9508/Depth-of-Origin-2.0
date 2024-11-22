using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpike : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float damage;
    [SerializeField] private float inactiveTime;
    [SerializeField] private float activeTime;

    [Header("Reference")]
    [SerializeField] private Animator animator;

    //Runtime data
    private float timeElapse;
    private bool isActive = false;
    private static bool canDamage = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Activiting();

        if (canDamage && isActive && DetectPlayer())
        {
            IDamageable damageableObject = GameObject.FindWithTag("Player").GetComponent<IDamageable>();

            damageableObject.OnHit(damage, false, Vector2.zero, 0);

            StartCoroutine(SetStaticTimer(callback => { canDamage = callback; }, 1));
        }
    }

    private bool DetectPlayer()
    {
        List<Collider2D> colliderResult = new();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), new(), colliderResult);

        bool isPlayerInRange = false;
        for (int i = 0; i < colliderResult.Count; i++)
        {
            if (colliderResult[i] != null && colliderResult[i].CompareTag("Player")) isPlayerInRange = true;
        }

        return isPlayerInRange;
    }

    private void Activiting()
    {
        timeElapse += Time.deltaTime;
        animator.SetBool("isActive", isActive);

        if (timeElapse >= inactiveTime && !isActive)
        {
            isActive = true;
            timeElapse = 0;
        }
        else if (timeElapse >= activeTime && isActive)
        {
            isActive = false;
            timeElapse = 0;
        }
    }

    private IEnumerator SetStaticTimer(System.Action<bool> callback, float time)
    {
        callback(false);
        yield return new WaitForSeconds(time);
        callback(true);
    }
}
