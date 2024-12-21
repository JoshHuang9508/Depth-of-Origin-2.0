using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [Header("Settings")]
    public string[] dialog = new string[] { };

    //Runtime data
    private PlayerBehaviour player;

    private void Update()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }
    public void Chat()
    {
        player.PlayDialog(name, dialog);
    }
}
