using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour
{
    private RoomManager roomTemplate;

    void Start()
    {
        roomTemplate = GameObject.FindWithTag("RoomTemplates").GetComponent<RoomManager>();
        roomTemplate.rooms.Add(this.gameObject);
    }
}
