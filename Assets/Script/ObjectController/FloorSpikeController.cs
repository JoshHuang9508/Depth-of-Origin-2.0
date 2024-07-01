using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikeController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float damage;
    [SerializeField] private float inactiveTime;
    [SerializeField] private float activeTime;

    [Header("Object Reference")]
    [SerializeField] private Animator animator;

    [Header("Dynamic Data")]
    [SerializeField] private float timeElapse;
    [SerializeField] private bool isActive = false;
    [SerializeField] private static bool damageEnablerStatic = true;



    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Activiting();

        if (damageEnablerStatic && isActive && DetectPlayer())
        {
            Damageable damageableObject = GameObject.FindWithTag("Player").GetComponent<Damageable>();

            damageableObject.OnHit(damage, false, Vector2.zero, 0);

            StartCoroutine(SetStaticTimer(callback => { damageEnablerStatic = callback; }, 1));
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
