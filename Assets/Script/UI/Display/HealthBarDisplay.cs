using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarDisplay : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private Gradient gradient;

    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image fill;
    [SerializeField] private PlayerBehaviour player;

    private void Update()
    {
        if (player != null)
        {
            SetDisplay();
        }
    }

    private void SetDisplay()
    {
        slider.value = player.Health / player.MaxHealth;
        healthText.text = $"{Mathf.RoundToInt(player.Health)} / {player.MaxHealth}";
        fill.color = gradient.Evaluate(player.Health / player.MaxHealth);
    }
}
