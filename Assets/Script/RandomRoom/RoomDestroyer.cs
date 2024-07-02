using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherColider)
    {
        if (otherColider.CompareTag("Water") || otherColider.CompareTag("Wall"))
        {
            Destroy(otherColider.transform.parent.gameObject);
        }
    }
}
