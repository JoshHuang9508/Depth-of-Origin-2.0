using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [Header("Attributes")]
    [SerializeField] private float maxHealth;

    [Header("Lootings")]
    [SerializeField] private List<Lootings> coins;
    [SerializeField] private List<Lootings> lootings;

    [Header("Audios")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip breakSound;

    // Status
    public float health { get; private set; }

    // Timer 
    public enum TimerType { Damage }
    private Dictionary<TimerType, float> timerList = new()
    {
        { TimerType.Damage, 0 }
    };

    // Rumtime Data
    private PlayerBehaviour player;

    private void Update()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        UpdateTimer();
    }

    ///////////////
    // Abilities //
    ///////////////

    private void Heal(float value)
    {
        health += Mathf.Min(value, maxHealth - health);
    }
    private void Damage(float value)
    {
        health -= value;
        // SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        AudioPlayer.PlaySound(hitSound);
    }
    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 kbForce, float kbTime)
    {
        if (!IsTimerEnd(TimerType.Damage) || attackerType != AttackerType.player) return;

        Damage(damage);

        SetTimer(TimerType.Damage, 0.2f);
    }

    ////////////
    // Update //
    ////////////

    private void UpdateTimer()
    {
        List<TimerType> keys = new(timerList.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            TimerType timerType = keys[i];
            timerList[timerType] -= Time.deltaTime;
        }
    }

    ////////////////
    // Properties //
    ////////////////

    public void KillObject()
    {
        ItemDropper.Drop(transform.position, lootings);
        ItemDropper.Drop(transform.position, coins);
        AudioPlayer.PlaySound(breakSound);
        Destroy(gameObject);
    }
    public void SetTimer(TimerType timerType, float value)
    {
        timerList[timerType] = value;
    }
    public bool IsTimerEnd(TimerType timerType)
    {
        return timerList[timerType] <= 0;
    }

}
