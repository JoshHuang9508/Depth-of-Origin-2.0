using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthWarning : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient gradient;

    [Header("Object Reference")]
    [SerializeField] private Image fill;
    [SerializeField] private PlayerBehaviour player;


    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        fill.color = gradient.Evaluate(player.Health / player.MaxHealth);
    }
}
