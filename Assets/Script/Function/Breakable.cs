using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Breakable : MonoBehaviour, Damageable
{
    [Header("Setting")]
    [SerializeField] private float health;

    [Header("Looting")]
    public List<Coins> coins;
    public List<Lootings> lootings;
    public List<GameObject> wreckage;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip breakSound;

    [Header("Reference")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private GameObject itemDropper;

    //Rumtime Data
    private PlayerBehaviour player;
    private bool damageEnabler = true;
    private float damageDisableTimer = 0;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Max(0, value);

            if (health <= 0)
            {
                KillObject();
            }
        }
        
    }

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
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

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (damageEnabler)
        {
            //update heath
            Health -= damage;

            //instantiate damege text
            player.SetDamageText(transform.position, damage, isCrit ? DamageTextDisplay.DamageTextType.DamageCrit : DamageTextDisplay.DamageTextType.Damage);

            //play audio
            audioPlayer.PlayOneShot(hitSound);

            //set timer
            damageDisableTimer += 0.2f;
        }
    }

    private void UpdateTimer()
    {
        //update timer
        damageDisableTimer = Mathf.Max(0, damageDisableTimer - Time.deltaTime);

        damageEnabler = damageDisableTimer <= 0;
    }

    private void KillObject()
    {
        //drop items
        ItemDropper ItemDropper = Instantiate(
            itemDropper,
            transform.position,
            Quaternion.identity,
            GameObject.FindWithTag("Item").transform
            ).GetComponent<ItemDropper>();

        ItemDropper.DropItems(lootings);
        ItemDropper.DropCoins(coins);
        ItemDropper.DropWrackages(wreckage);

        //play audio
        audioPlayer.PlayOneShot(breakSound);

        Destroy(gameObject);
    }
}
