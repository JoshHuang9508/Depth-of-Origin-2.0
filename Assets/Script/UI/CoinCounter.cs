using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private TMP_Text coinCounterText;
    [SerializeField] private PlayerBehaviour player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        coinCounterText.text = $"x{player.CoinAmount}";
    }
}
