using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PoisonController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private EdibleItemSO poisonEffect;
    [SerializeField] private float aliveTime = 10f;

    [Header("Dynamic Data")]
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private static bool damageEnablerStatic = true;


    
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= aliveTime && aliveTime != 0)
        {
            Destroy(gameObject);
        }

        if(DetectPlayer() && damageEnablerStatic)
        {
            Damageable damageableObject = GameObject.FindWithTag("Player").GetComponent<Damageable>();

            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().SetEffection(poisonEffect);
            damageableObject.OnHit(10, false, Vector2.zero, 0);

            StartCoroutine(SetTimer(callback => { damageEnablerStatic = callback; }, 2));
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

    private IEnumerator SetTimer(System.Action<bool> callback, float time)
    {
        callback(false);
        yield return new WaitForSeconds(time);
        callback(true);
    }

    public void PoisonSetup(float aliveTime)
    {
        this.aliveTime = aliveTime;
    }
}
