using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChecker : MonoBehaviour
{
    public bool isWall = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) isWall = true;
    }
}
