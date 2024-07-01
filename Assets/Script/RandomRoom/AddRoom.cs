using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour
{
    private RoomTemplate roomTemplate;

    void Start()
    {
        roomTemplate = GameObject.FindWithTag("RoomTemplates").GetComponent<RoomTemplate>();
        roomTemplate.rooms.Add(this.gameObject);
    }
}
