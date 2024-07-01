using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [Header("Setting")]
    public List<Vector2> positionList;

    [Header("Object Reference")]
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject column;

    [Header("Dynamic Data")]
    int behaviorType = 1;


    void Start()
    {
        column.GetComponent<ShieldHolderController>().shieldBreak += RemoveShield;

        enemyBehavior.currentRb.bodyType = RigidbodyType2D.Static;
        enemyBehavior.movementDisableTimer = 3;
        enemyBehavior.damageDisableTimer = 3;
        enemyBehavior.attackDisableTimer = 5;

        StartCoroutine(SetTimer(callback => {
            enemyBehavior.behaviourEnabler = callback;
        }, 5f));

        BuildColumns();
    }

    private void Update()
    {
        shield.SetActive(enemyBehavior.HaveShield);

        if(enemyBehavior.Health <= enemyBehavior.enemy.health * 0.5 && behaviorType == 1)
        {
            enemyBehavior.movementDisableTimer = 3;
            enemyBehavior.damageDisableTimer = 3;
            enemyBehavior.attackDisableTimer = 5;
            behaviorType = 2;
        }

        if (Mathf.RoundToInt(enemyBehavior.attackDisableTimer) == 2)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().PlayAnimator("Warning");
        }

        if (!enemyBehavior.behaviourEnabler) return;

        switch (behaviorType)
        {
            case 1:
                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Static;
                enemyBehavior.enemy.walkType = EnemySO.WalkType.None;
                enemyBehavior.enemy.attackType = EnemySO.AttackType.Sniper;
                enemyBehavior.enemy.attackField = 100;
                enemyBehavior.enemy.chaseField = 0;
                enemyBehavior.enemy.attackSpeed = 0.4f;
                enemyBehavior.enemy.attackDamage = 1500;
                break;

            case 2:
                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemyBehavior.enemy.walkType = EnemySO.WalkType.Melee;
                enemyBehavior.enemy.attackType = EnemySO.AttackType.Melee;
                enemyBehavior.enemy.attackField = 3f;
                enemyBehavior.enemy.chaseField = 100;
                enemyBehavior.enemy.attackSpeed = 1;
                enemyBehavior.enemy.attackDamage = 2000;
                break;
        }
    }

    public void RemoveShield()
    {
        enemyBehavior.ShieldHealth = 0;
        enemyBehavior.attackDisableTimer = 65f;

        StartCoroutine(SetTimer((callback) =>{
            if (callback && behaviorType == 1)
            {
                enemyBehavior.SetShield();
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
}
