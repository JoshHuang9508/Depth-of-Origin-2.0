using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGeneration;

public class RoomSpawner : MonoBehaviour
{
    [Header("Setting")]
    public OpeningDirection openingDirection;
    [SerializeField] private float existTime = 5f;
    [SerializeField] private int index;

    [Header("Reference")]
    [SerializeField] private RoomChecker topRoomChecker; // 1 -> Wall | 0 -> None
    [SerializeField] private RoomChecker leftRoomChecker;
    [SerializeField] private RoomChecker bottomRoomChecker;
    [SerializeField] private RoomChecker rightRoomChecker;

    private RoomTemplate template;
    private int rand;
    private bool spawned = false;
    [SerializeField] private int collisionRoomIndex = 0;

    public enum OpeningDirection
    {
        Top, Left, Bottom, Right
    }

    void Start()
    {
        template = GameObject.FindWithTag("RoomTemplates").GetComponent<RoomTemplate>();

        Invoke(nameof(CheckWall), 0.1f);
        Invoke(nameof(SpawnRoom), 0.1f);
        Destroy(gameObject, existTime);
    }

    private void CheckWall()
    {
        collisionRoomIndex = (topRoomChecker.isWall ? 0 : 8) + (leftRoomChecker.isWall ? 0 : 4) + (bottomRoomChecker.isWall ? 0 : 2) + (rightRoomChecker.isWall ? 0 : 1);
        // 1001(2) means the room which is gonna to spawn cant have left and bottom door
    }

    private void SpawnRoom()
    {
        if(!spawned)
        {
            switch (openingDirection)
            {
                case OpeningDirection.Top:
                    //need to spawn a room with a BOTTOM door
                    rand = UnityEngine.Random.Range(0, template.bottomRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.bottomRooms[rand].chance && ((template.bottomRooms[rand].index & collisionRoomIndex) == template.bottomRooms[rand].index))
                    {
                        Instantiate(template.bottomRooms[rand].room, transform.position, template.bottomRooms[rand].room.transform.rotation, transform.parent.parent);
                    }
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Left:
                    //need to spawn a room with a RIGHT door
                    rand = UnityEngine.Random.Range(0, template.rightRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.rightRooms[rand].chance && ((template.rightRooms[rand].index & collisionRoomIndex) == template.rightRooms[rand].index))
                    {
                        Instantiate(template.rightRooms[rand].room, transform.position, template.rightRooms[rand].room.transform.rotation, transform.parent.parent);
                    }
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Bottom:
                    //need to spawn a room with a TOP door
                    rand = UnityEngine.Random.Range(0, template.topRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.topRooms[rand].chance && ((template.topRooms[rand].index & collisionRoomIndex) == template.topRooms[rand].index))                    {
                        Instantiate(template.topRooms[rand].room, transform.position, template.topRooms[rand].room.transform.rotation, transform.parent.parent);
                    }
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Right:
                    //need to spawn a room with a LEFT door
                    rand = UnityEngine.Random.Range(0, template.leftRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.leftRooms[rand].chance && ((template.leftRooms[rand].index & collisionRoomIndex) == template.leftRooms[rand].index))
                    {
                        Instantiate(template.leftRooms[rand].room, transform.position, template.leftRooms[rand].room.transform.rotation, transform.parent.parent);
                    }
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
            }
            spawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D otherColider)
    {
        //if (otherColider.CompareTag("RoomStart")) Destroy(gameObject);

        if (otherColider.CompareTag("RoomSpawnPoint"))
        {
            try
            {
                if (!otherColider.GetComponent<RoomSpawner>().spawned && !spawned)
                {
                    //create wall room
                    Instantiate(template.wallRoom, transform.position, template.wallRoom.transform.rotation, transform.parent.parent);
                    Destroy(gameObject);
                }
                spawned = true;
            }
            catch { }
        }
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