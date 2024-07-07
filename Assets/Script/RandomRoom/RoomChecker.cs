using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChecker : MonoBehaviour
{
    public bool isWall = false;
    public bool isDoor = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) isWall = true;
        if (other.CompareTag("Ground") && !isWall) isDoor = true;
    }

    private void Update()
    {
        if (isWall) isDoor = false;
    }
}
