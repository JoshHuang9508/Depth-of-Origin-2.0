using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthWarningDisplay : MonoBehaviour
{
    [Header("Attributes")]
    public Gradient gradient;

    [Header("References")]
    [SerializeField] private Image fill;
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
        fill.color = gradient.Evaluate(player.health / player.MaxHealth);
    }
}
