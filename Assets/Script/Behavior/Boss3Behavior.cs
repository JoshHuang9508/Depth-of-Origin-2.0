using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Behavior : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private GameObject firstProjectile;
    [SerializeField] private GameObject seconedProjectile;

    [Header("Setting")]
    [SerializeField] private float disableTimer = 10;
    [SerializeField] private int behaviorType = 1;
    

    private void Start()
    {
        enemyBehavior.OnAttack += Attacking;

        enemyBehavior.currentRb.bodyType = RigidbodyType2D.Static;
        enemyBehavior.movementDisableTimer = 3;
        enemyBehavior.damageDisableTimer = 3;
        enemyBehavior.attackDisableTimer = 13;

        StartCoroutine(SetTimer(callback => {
            enemyBehavior.behaviourEnabler = callback;
        }, 5f));
    }

    private void Update()
    {
        if (enemyBehavior.Health <= enemyBehavior.enemy.health * 0.5 && behaviorType == 1)
        {
            enemyBehavior.movementDisableTimer = 3;
            enemyBehavior.damageDisableTimer = 3;
            enemyBehavior.attackDisableTimer = 5;
            behaviorType = 2;
        }

        if (!enemyBehavior.behaviourEnabler) return;

        if (Mathf.RoundToInt(enemyBehavior.attackDisableTimer) == 2)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().PlayAnimator("Warning");
        }

        switch (behaviorType)
        {
            case 1:
                
                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemyBehavior.enemy.shootingType = EnemySO.ShootingType.AllAngle;
                enemyBehavior.enemy.moveSpeed = 2;
                enemyBehavior.enemy.attackSpeed = 10;
                enemyBehavior.enemy.projectile = firstProjectile;

                if (enemyBehavior.HaveShield)
                {
                    disableTimer = 10;
                }
                else if (!enemyBehavior.HaveShield)
                {
                    enemyBehavior.movementDisableTimer = 3;
                    enemyBehavior.attackDisableTimer = 13;

                    disableTimer -= Time.deltaTime;

                    enemyBehavior.ShieldHealth = enemyBehavior.enemy.shieldHealth * (1 - Mathf.Max(disableTimer / 10, 0));

                    if (disableTimer <= 0) enemyBehavior.SetShield();
                }
                break;

            case 2:

                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemyBehavior.enemy.shootingType = EnemySO.ShootingType.Single;
                enemyBehavior.enemy.moveSpeed = 6;
                enemyBehavior.enemy.attackSpeed = 0.6f;
                enemyBehavior.enemy.projectile = seconedProjectile;
                enemyBehavior.ShieldHealth = 0;

                break;
        }
    }

    private void Attacking()
    {
        enemyBehavior.ShieldHealth -= enemyBehavior.enemy.shieldHealth * 0.25f;
    }

    private IEnumerator SetTimer(System.Action<bool> callback, float time)
    {
        callback(false);
        yield return new WaitForSeconds(time);
        callback(true);
    }
}
