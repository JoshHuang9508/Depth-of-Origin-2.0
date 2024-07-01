using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Vector3 chestSummonPos;
    [SerializeField] private Vector3 exitSummonPos;
    [SerializeField] private List<Coins> coins;
    [SerializeField] private List<Lootings> lootings;
    [SerializeField] private int sceneNum;
 
    [Header("Object Reference")]
    [SerializeField] private GameObject spawner;
    [SerializeField] private GameObject chest;
    [SerializeField] private GameObject exit;

    [Header("Dynamic Data")]
    [SerializeField] private bool bossAlive = false;
    [SerializeField] private int bossCounter = 0;
    [SerializeField] private List<GameObject> entitylist = new();
    

    private void Start()
    {
        bossAlive = false;
    }

    private void Update()
    {
        bossCounter = 0;
        entitylist = new();

        int entitycount = GameObject.Find("Entity").transform.childCount;

        for (int i = 0; i < entitycount; i++)
        {
            entitylist.Add(GameObject.Find("Entity").transform.GetChild(i).gameObject);
        }

        foreach (var entity in entitylist)
        {
            //need fix lol
            if (entity.GetComponent<BossBehavior>() != null || entity.GetComponent<Boss2Behavior>() != null || entity.GetComponent<Boss3Behavior>() != null)
            {
                bossCounter++;
            }
        }

        if(bossAlive && bossCounter == 0)
        {
            ChestController chestSummoned = Instantiate(
                chest, 
                chestSummonPos, 
                Quaternion.identity, 
                GameObject.Find("Object_Grid").transform).GetComponent<ChestController>();
            chestSummoned.SetChestContent(coins, lootings);

            SceneLoaderController exitSummoned = Instantiate(
                exit,
                exitSummonPos,
                Quaternion.identity,
                GameObject.Find("Object_Grid").transform).GetComponent<SceneLoaderController>();
            exitSummoned.SetSceneLoaderContent(sceneNum);

            bossAlive = false;
        }
    }

    public void SummonBoss()
    {
        if (!bossAlive)
        {
            spawner.GetComponent<SpawnerController>().SpawnMobs();
            bossAlive = true;
        }
    }
}
