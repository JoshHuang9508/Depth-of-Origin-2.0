using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGeneration;
using System.Threading.Tasks;

public class RoomSpawner : MonoBehaviour
{
    [Header("Setting")]
    public OpeningDirection openingDirection;
    public float existTime = 10f;
    [SerializeField] private int index;

    [Header("Reference")]
    [SerializeField] private RoomChecker topRoomChecker; // 1 -> Wall | 0 -> None
    [SerializeField] private RoomChecker leftRoomChecker;
    [SerializeField] private RoomChecker bottomRoomChecker;
    [SerializeField] private RoomChecker rightRoomChecker;

    private RoomManager roomTemplate;
    private int rand;
    private int triedTimes = 0;
    [SerializeField] private bool spawned = false;
    [SerializeField] private int collisionWallIndex = 0;
    [SerializeField] private int collisionDoorIndex = 0;


    public enum OpeningDirection
    {
        Top, Left, Bottom, Right
    }

    private void Start()
    {
        roomTemplate = GameObject.FindWithTag("RoomTemplates").GetComponent<RoomManager>();
        roomTemplate.RoomSpawners.Add(this);
    }

    private void CheckWall()
    {
        collisionWallIndex = (topRoomChecker.isWall ? 0 : 8) + (leftRoomChecker.isWall ? 0 : 4) + (bottomRoomChecker.isWall ? 0 : 2) + (rightRoomChecker.isWall ? 0 : 1);
        // 1001(2) means the room which is gonna to spawn cant have left and bottom door
    }

    private void CheckDoor()
    {
        collisionDoorIndex = (topRoomChecker.isDoor ? 8 : 0) + (leftRoomChecker.isDoor ? 4 : 0) + (bottomRoomChecker.isDoor ? 2 : 0) + (rightRoomChecker.isDoor ? 1 : 0);
        // 1001(2) means the room which is gonna to spawn must have top and right door
    }

    private bool CheckRoom()
    {
        Collider2D[] otherColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5f, 0.5f), 0f);

        if (otherColliders.Length != 1)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> SpawnRoom()
    {
        while (!spawned && triedTimes < 100)
        {
            CheckWall();
            CheckDoor();
            if (!CheckRoom())
            {
                return false;
            }

            Debug.Log($"Wall index : {collisionWallIndex} | Door index : {collisionDoorIndex}");

            switch (openingDirection)
            {
                case OpeningDirection.Top:
                    //need to spawn a room with a BOTTOM door
                    rand = UnityEngine.Random.Range(0, roomTemplate.bottomRooms.Count);
                    Debug.Log($"Random index : {roomTemplate.bottomRooms[rand].index}");

                    if (UnityEngine.Random.Range(0, 100) <= roomTemplate.bottomRooms[rand].chance && 
                        ((roomTemplate.bottomRooms[rand].index & collisionWallIndex) == roomTemplate.bottomRooms[rand].index) &&
                        ((roomTemplate.bottomRooms[rand].index | collisionDoorIndex) == roomTemplate.bottomRooms[rand].index))
                    {
                        Instantiate(roomTemplate.bottomRooms[rand].room, transform.position, roomTemplate.bottomRooms[rand].room.transform.rotation, transform.parent.parent);
                        spawned = true;
                    }
                    break;

                case OpeningDirection.Left:
                    //need to spawn a room with a RIGHT door
                    rand = UnityEngine.Random.Range(0, roomTemplate.rightRooms.Count);
                    Debug.Log($"Random index : {roomTemplate.rightRooms[rand].index}");

                    if (UnityEngine.Random.Range(0, 100) <= roomTemplate.rightRooms[rand].chance && 
                        ((roomTemplate.rightRooms[rand].index & collisionWallIndex) == roomTemplate.rightRooms[rand].index) &&
                        ((roomTemplate.rightRooms[rand].index | collisionDoorIndex) == roomTemplate.rightRooms[rand].index))
                    {
                        Instantiate(roomTemplate.rightRooms[rand].room, transform.position, roomTemplate.rightRooms[rand].room.transform.rotation, transform.parent.parent);
                        spawned = true;
                    }
                    break;

                case OpeningDirection.Bottom:
                    //need to spawn a room with a TOP door
                    rand = UnityEngine.Random.Range(0, roomTemplate.topRooms.Count);
                    Debug.Log($"Random index : {roomTemplate.topRooms[rand].index}");

                    if (UnityEngine.Random.Range(0, 100) <= roomTemplate.topRooms[rand].chance && 
                        ((roomTemplate.topRooms[rand].index & collisionWallIndex) == roomTemplate.topRooms[rand].index) &&
                        ((roomTemplate.topRooms[rand].index | collisionDoorIndex) == roomTemplate.topRooms[rand].index))
                    {
                        Instantiate(roomTemplate.topRooms[rand].room, transform.position, roomTemplate.topRooms[rand].room.transform.rotation, transform.parent.parent);
                        spawned = true;
                    }
                    break;

                case OpeningDirection.Right:
                    //need to spawn a room with a LEFT door
                    rand = UnityEngine.Random.Range(0, roomTemplate.leftRooms.Count);
                    Debug.Log($"Random index : {roomTemplate.leftRooms[rand].index}");

                    if (UnityEngine.Random.Range(0, 100) <= roomTemplate.leftRooms[rand].chance && 
                        ((roomTemplate.leftRooms[rand].index & collisionWallIndex) == roomTemplate.leftRooms[rand].index) &&
                        ((roomTemplate.leftRooms[rand].index | collisionDoorIndex) == roomTemplate.leftRooms[rand].index))
                    {
                        Instantiate(roomTemplate.leftRooms[rand].room, transform.position, roomTemplate.leftRooms[rand].room.transform.rotation, transform.parent.parent);
                        spawned = true;
                    }
                    break;
            }

            triedTimes++;
            switch (triedTimes)
            {
                case 20:
                    Debug.LogWarning("More than 20 tries!");
                    break;
                case 40:
                    Debug.LogWarning("More than 40 tries!");
                    break;
                case 60:
                    Debug.LogWarning("More than 60 tries!");
                    break;
                case 80:
                    Debug.LogWarning("More than 80 tries!");
                    break;
                case 100:
                    Debug.LogError("More than 100 tries!!! Force to stop.");
                    break;
            }
            //await Task.Delay(10);
        }
        return true;
    }
}

namespace RoomGeneration
{
    [Serializable]
    public struct RoomsWithChances
    {
        public GameObject room;
        public float chance;
        public int index;
    }
}