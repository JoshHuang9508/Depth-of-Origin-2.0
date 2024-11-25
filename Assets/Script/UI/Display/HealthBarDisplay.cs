using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarDisplay : MonoBehaviour
{
    [Header("Attributes")]
    public Gradient gradient;

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
        slider.value = player.health / player.MaxHealth;
        healthText.text = $"{Mathf.RoundToInt(player.health)} / {player.MaxHealth}";
        fill.color = gradient.Evaluate(player.health / player.MaxHealth);
    }
}
