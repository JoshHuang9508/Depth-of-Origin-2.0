using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomGeneration;

public class RoomTemplate : MonoBehaviour
{
    public List<RoomsWithChances> topRooms;
    public List<RoomsWithChances> leftRooms;
    public List<RoomsWithChances> bottomRooms;
    public List<RoomsWithChances> rightRooms;
    public GameObject wallRoom;

    public List<GameObject> rooms;
}
