using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float spawnRange;
    [SerializeField] private bool autoSpawn = true;
    [SerializeField] private float minSpawnDistance = 15;
    [SerializeField] private int mobsStayedLimit = 4;
    [SerializeField] private int spawnTimesLimit = -1;
    [SerializeField] private float spawnGap = 3;
    [SerializeField] private int trySpawnTimes = -1;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private List<EnemySO> spawnList;

    [Header("Dynamic Data")]
    [SerializeField] private int spawnTimes;
    [SerializeField] private int stayedMobs;
    [SerializeField] private float spawnTimer = 0;
    [SerializeField] private bool spawnEnabler = true;

    private void Start()
    {
        if (GetComponent<CircleCollider2D>())
        {
            spawnRange = GetComponent<CircleCollider2D>().radius;
        }
    }

    private void Update()
    {
        if (spawnEnabler && autoSpawn)
        {
            spawnTimer += Time.deltaTime;

            if(spawnTimer > spawnGap)
            {
                SpawnMobs();
                spawnTimer = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) stayedMobs++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) stayedMobs--;
    }

    public void SpawnMobs()
    {
        //detect spawn restrict
        if (spawnEnabler && (stayedMobs < mobsStayedLimit || mobsStayedLimit == -1) && (spawnTimesLimit > spawnTimes || spawnTimesLimit == -1) &&
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().behaviourEnabler)
        {
            spawnEnabler = false;

            Vector2 spawnPos = Vector2.zero;
            Vector2 playerPos = GameObject.FindWithTag("Player").transform.position;

            int tryTimes = 0;

            //detect spawn position
            while (DetectBlankAreas(spawnPos, new Vector2(2f, 2f), 0.1f) || spawnPos == Vector2.zero)
            {
                //random spawn position
                float spawnX = Random.Range(-1 * spawnRange, spawnRange);
                float spawnY = Random.Range(-1 * Mathf.Sqrt((spawnRange * spawnRange) - (spawnX * spawnX)), Mathf.Sqrt((spawnRange * spawnRange) - (spawnX * spawnX)));
                spawnPos = new Vector2(
                        transform.position.x + spawnX,
                        transform.position.y + spawnY);

                tryTimes++;
                if (tryTimes >= trySpawnTimes && trySpawnTimes != -1)
                {
                    spawnEnabler = true;
                    return;
                }
            }

            //detect player distance
            if (Vector2.Distance(playerPos, spawnPos) < minSpawnDistance)
            {
                spawnEnabler = true;
                return;
            }

            //spawn mobs
            int randomSpawnIndex = Random.Range(0, spawnList.Count);
            var spawnMob = Instantiate(
                spawnList[randomSpawnIndex].EnemyObject,
                spawnPos,
                Quaternion.identity,
                GameObject.FindWithTag("Entity").transform);
            spawnMob.GetComponent<EnemyBehavior>().enemy = spawnList[randomSpawnIndex];
            spawnTimes++;

            //spawn delay
            spawnEnabler = true;
        }
    }

    private bool DetectBlankAreas(Vector2 areaCenter, Vector2 areaSize, float cellSize)
    {
        for (float x = areaCenter.x - areaSize.x / 2; x < areaCenter.x + areaSize.x / 2; x += cellSize)
        {
            for (float y = areaCenter.y - areaSize.y / 2; y < areaCenter.y + areaSize.y / 2; y += cellSize)
            {
                Vector2 cellPosition = new(x, y);
                Collider2D[] colliders = Physics2D.OverlapBoxAll(cellPosition, new Vector2(cellSize, cellSize), 0f, targetLayer);

                if (colliders.Length == 0) return true;
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Water") || collider.CompareTag("HitBox") || collider.CompareTag("BreakableObject") || collider.CompareTag("Wall") || collider.CompareTag("Object")) return true;
                }
            }
        }

        return false;
    }
}
