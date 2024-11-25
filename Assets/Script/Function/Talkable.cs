using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [Header("Attributes")]
    public string[] dialog = new string[] { };

    //Runtime data
    private PlayerBehaviour player;

    void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by talkable.cs)");
        }
    }

    public async void Chat()
    {
        await player.SetDialog(dialog);
    }
}
