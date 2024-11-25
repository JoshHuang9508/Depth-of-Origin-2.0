using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text coinCounterText;
    [SerializeField] private PlayerBehaviour player;

    void Update()
    {
        if (player != null)
        {
            SetDisplay();
        }
    }

    private void SetDisplay()
    {
        coinCounterText.text = $"x{player.coinAmount}";
    }
}
