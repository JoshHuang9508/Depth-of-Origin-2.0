using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarDisplay : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient gradient;

    [Header("Object Reference")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image fill;

    //Runtime data
    private PlayerBehaviour player;

    private void Update()
    {
        //Get player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by HeathBarDisplay.cs)");
        }

        if(player != null)
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
