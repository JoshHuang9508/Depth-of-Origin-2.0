using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Inventory.Model;

public class Breakable : MonoBehaviour, Damageable
{
    [Header("Setting")]
    [SerializeField] private float health;
    [SerializeField] private List<Coins> coins;
    [SerializeField] private List<Lootings> lootings;
    [SerializeField] private List<GameObject> wreckage;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip breakSound;

    [Header("Object Reference")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject itemDropper;

    [Header("Dynamic Data")]
    public bool damageEnabler = true;
    public float damageDisableTimer = 0;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;

            if (health <= 0)
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
        
    }

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateTimer();
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (damageEnabler)
        {
            //update heath
            Health -= damage;

            //instantiate damege text
            DamageText.InstantiateDamageText(damageText, transform.position, damage, isCrit ? "DamageCrit" : "Damage");

            //play audio
            audioPlayer.PlayOneShot(hitSound);

            //set timer
            damageDisableTimer += 0.2f;
        }
    }

    public void UpdateTimer()
    {
        //update timer
        damageDisableTimer = Mathf.Max(0, damageDisableTimer - Time.deltaTime);

        damageEnabler = damageDisableTimer <= 0;
    }

    
}
