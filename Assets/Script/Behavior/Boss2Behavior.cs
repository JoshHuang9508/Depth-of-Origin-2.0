using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Boss2Behavior : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private EnemyBehavior enemyBehavior;
    [SerializeField] private GameObject poison;
    [SerializeField] private EnemySO smallerSlime;

    [Header("Setting")]
    [SerializeField] private SlimeSplitStage slimeSplitStage;
    [SerializeField] private enum SlimeSplitStage
    {
        Big, Mid, Small
    }


    private void Update()
    {
        switch (slimeSplitStage)
        {
            case SlimeSplitStage.Big:

                float distant = Vector3.Distance(enemyBehavior.CurrentPos, enemyBehavior.TargetPos);

                if (distant > 6f)
                {
                    enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                    enemyBehavior.enemy.attackType = EnemySO.AttackType.Sniper;
                    enemyBehavior.enemy.attackField = 100;
                    enemyBehavior.enemy.chaseField = 100;
                    enemyBehavior.enemy.knockbackForce = 30f;

                    poison.SetActive(false);
                }
                else if (distant <= 6f)
                {
                    enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                    enemyBehavior.enemy.attackType = EnemySO.AttackType.Melee;
                    enemyBehavior.enemy.attackField = 3f;
                    enemyBehavior.enemy.chaseField = 100;
                    enemyBehavior.enemy.knockbackForce = 100f;

                    poison.SetActive(true);
                }
                break;

            case SlimeSplitStage.Mid:

                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemyBehavior.enemy.attackType = EnemySO.AttackType.Melee;
                enemyBehavior.enemy.attackField = 2;
                enemyBehavior.enemy.chaseField = 100;
                enemyBehavior.enemy.knockbackForce = 30f;

                break;

            case SlimeSplitStage.Small:

                enemyBehavior.currentRb.bodyType = RigidbodyType2D.Dynamic;
                enemyBehavior.enemy.attackType = EnemySO.AttackType.Melee;
                enemyBehavior.enemy.attackField = 1.5f;
                enemyBehavior.enemy.chaseField = 100;
                enemyBehavior.enemy.knockbackForce = 15f;

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
