using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [Header("Datas")]

    [Header("Attributes")]
    [SerializeField] private float maxHealth;

    [Header("Lootings")]
    [SerializeField] private List<Coins> coins;
    [SerializeField] private List<Lootings> lootings;
    [SerializeField] private List<GameObject> wreckage;

    [Header("Audios")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip breakSound;

    [Header("References")]
    [SerializeField] private GameObject itemDropper;

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
        // AudioPlayer.Playsound(hitSound);
    }
    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
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
        foreach (TimerType timerType in timerList.Keys)
        {
            timerList[timerType] -= Time.deltaTime;
        }
    }

    ////////////////
    // Properties //
    ////////////////

    private void KillObject()
    {
        // drop items -> change this to a function
        ItemDropper ItemDropper = Instantiate(
            itemDropper,
            transform.position,
            Quaternion.identity,
            GameObject.FindWithTag("Item").transform
            ).GetComponent<ItemDropper>();

        ItemDropper.DropItems(lootings);
        ItemDropper.DropCoins(coins);
        ItemDropper.DropWrackages(wreckage);

        // ItemDropper.DropItem();
        // AudioPlayer.Playsound(breakSound);

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
