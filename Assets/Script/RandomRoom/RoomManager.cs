using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGeneration;
using System.Threading.Tasks;

public class RoomManager : MonoBehaviour
{
    [Header("Room Setting")]
    public List<RoomsWithChances> topRooms;
    public List<RoomsWithChances> leftRooms;
    public List<RoomsWithChances> bottomRooms;
    public List<RoomsWithChances> rightRooms;
    public GameObject wallRoom;

    public List<GameObject> rooms;

    [Header("Boss Room Setting")]
    public EnemySO boss;

    [Header("Reference")]
    [SerializeField] private GameObject teleportEntry;
    [SerializeField] private DungeonScene dungeonScene;

    public List<RoomSpawner> RoomSpawners = new();


    private async void Start()
    {
        dungeonScene.isRoomGenerateDone = false;
        await Task.Delay(20);
        await CheckRoomSpawn();
        await SummonBossRoom();
        dungeonScene.isRoomGenerateDone = true;
        Debug.Log("Load Done");
    }

    private async Task SummonBossRoom()
    {
        for(int i = 1; i < rooms.Count - 1; i++)
        {
            if (rooms[^i] != null)
            {
                Spawner.SpawnMob(rooms[^i].transform.position, boss);

                var _teleportEntry = Instantiate(
                    teleportEntry,
                    rooms[^i].transform.position + new Vector3(1.5f, 0.5f),
                    Quaternion.identity,
                    GameObject.FindWithTag("Object").transform).GetComponent<SceneLoader>();
                _teleportEntry.SetSceneLoaderContent(GameObject.FindWithTag("RoomStart"));

                break;
            }
        }
    }

    private async Task CheckRoomSpawn()
    {
        while(RoomSpawners.Count != 0)
        {
            bool isSpawnSuccess = await RoomSpawners[0].SpawnRoom();
            Destroy(RoomSpawners[0].gameObject);
            RoomSpawners.RemoveAt(0);
            await Task.Delay(10);
        }
    }
}