using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGeneration;

public class RoomSpawner : MonoBehaviour
{
    public OpeningDirection openingDirection;
    public float waitTime = 1f;

    private RoomTemplate template;
    private int rand;
    private bool spawned = false;

    public enum OpeningDirection
    {
        Top, Left, Bottom, Right
    }

// Start is called before the first frame update
void Start()
    {
        template = GameObject.FindWithTag("RoomTemplates").GetComponent<RoomTemplate>();

        Invoke(nameof(SpawnRoom), 0.1f);
        Destroy(gameObject, waitTime);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    if (UnityEngine.Random.Range(0, 100) <= template.bottomRooms[rand].chance) Instantiate(template.bottomRooms[rand].room, transform.position, template.bottomRooms[rand].room.transform.rotation, transform.parent.parent);
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Left:
                    //need to spawn a room with a RIGHT door
                    rand = UnityEngine.Random.Range(0, template.rightRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.rightRooms[rand].chance) Instantiate(template.rightRooms[rand].room, transform.position, template.rightRooms[rand].room.transform.rotation, transform.parent.parent);
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Bottom:
                    //need to spawn a room with a TOP door
                    rand = UnityEngine.Random.Range(0, template.topRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.topRooms[rand].chance) Instantiate(template.topRooms[rand].room, transform.position, template.topRooms[rand].room.transform.rotation, transform.parent.parent);
                    else
                    {
                        SpawnRoom();
                        return;
                    }
                    break;
                case OpeningDirection.Right:
                    //need to spawn a room with a LEFT door
                    rand = UnityEngine.Random.Range(0, template.leftRooms.Count);
                    if (UnityEngine.Random.Range(0, 100) <= template.leftRooms[rand].chance) Instantiate(template.leftRooms[rand].room, transform.position, template.leftRooms[rand].room.transform.rotation, transform.parent.parent);
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
    }
}