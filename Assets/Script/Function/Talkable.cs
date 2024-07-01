using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour player;
    [SerializeField] private string[] dialog = new string[] 
    { 
        "test",
        "test2"
    }; 

    void Update()
    {
        try { player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); } catch { }
    }

    public async void Chat()
    {
        await player.SetDialog(dialog);
    }
}
