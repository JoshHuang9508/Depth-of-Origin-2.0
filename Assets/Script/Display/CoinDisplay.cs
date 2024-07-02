using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TMP_Text coinCounterText;

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
            Debug.LogWarning("Can't find player (sent by CoinCounter.cs)");
        }

        coinCounterText.text = $"x{player.coinAmount}";
    }
}
