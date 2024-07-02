using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthWarningDisplay : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient gradient;

    [Header("Reference")]
    [SerializeField] private Image fill;

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
            Debug.LogWarning("Can't find player (sent by HealWarningDisplay.cs)");
        }

        fill.color = gradient.Evaluate(player.Health / player.MaxHealth);
    }
}
