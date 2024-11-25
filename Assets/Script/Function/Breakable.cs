using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    [Header("Status")]
    public bool canDamage = true;

    [Header("Settings")]
    [SerializeField] private float health;

    [Header("Lootings")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip breakSound;

    [Header("Reference")]
    [SerializeField] private GameObject itemDropper;

    //Rumtime Data
    private PlayerBehaviour player;

    private float DamageTimer
    {
        get
        {
            return DamageTimer;
        }
        set
        {
            DamageTimer = Mathf.Max(DamageTimer, value);
            canDamage = DamageTimer <= 0;
        }
    }
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Max(0, value);
            if (health <= 0) KillObject();
        }
    }

    private void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by Breakable.cs)");
        }

        UpdateTimer();
    }

    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!canDamage || attackerType != AttackerType.player) return;

        Health -= damage;
        //instantiate damege text
        // AudioPlayer.Playsound(hitSound);

        DamageTimer = 0.2f;
    }

    private void UpdateTimer()
    {
        DamageTimer = Mathf.Max(0, DamageTimer - Time.deltaTime);
    }

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
}
