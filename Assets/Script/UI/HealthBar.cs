using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient gradient;

    [Header("Object Reference")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image fill;
    [SerializeField] private PlayerBehaviour player;


    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        slider.value = player.Health / player.MaxHealth;
        healthText.text = $"{Mathf.RoundToInt(player.Health)} / {player.MaxHealth}";
        fill.color = gradient.Evaluate(player.Health / player.MaxHealth);
    }
}
