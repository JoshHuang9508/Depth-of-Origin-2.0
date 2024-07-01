using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;
using Inventory.UI;
using System;

public class PlayerBehaviour : MonoBehaviour, Damageable
{
    [Header("Player Stats")]
    [SerializeField] private float B_WalkSpeed;
    [SerializeField] private float B_MaxHealth;
    [SerializeField] private float B_Strength;
    [SerializeField] private float B_Defence;
    [SerializeField] private float B_CritRate;
    [SerializeField] private float B_CritDamage;
    [SerializeField] private List<Effection> effectionList = new();
    [SerializeField] private float E_WalkSpeed;
    [SerializeField] private float E_MaxHealth;
    [SerializeField] private float E_Strength;
    [SerializeField] private float E_Defence;
    [SerializeField] private float E_CritRate;
    [SerializeField] private float E_CritDamage;
    [SerializeField] private float currentHealth;
    [SerializeField] private int currentCoinAmount = 0;
    [SerializeField] private WeaponSO currentWeapon;
    [SerializeField] private List<Key> keyList = new();
    public int weaponControl = 0;

    [Header("Key Settings")]
    public KeyCode sprintKey;
    public KeyCode backpackKey;
    public KeyCode usePotionKey;
    public KeyCode useWeaponKey;
    public KeyCode meleeWeaponKey;
    public KeyCode rangedWeaponKey;
    public KeyCode interactKey;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;

    [Header("Object Reference")]
    public GameObject inventoryUI;
    public GameObject shopUI;
    public GameObject pauseUI;
    public GameObject deathUI;
    [SerializeField] private Animator camEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator warningAnim;
    [SerializeField] private SummonWeapon summonWeapon;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private GameObject damageText;
    [SerializeField] private GameObject itemDropper;

    [Header("Dynamic Data")]
    public InventorySO inventoryData;
    public InventorySO equipmentData;

    public bool behaviourEnabler = true;
    public bool movementEnabler = true;
    public float movementDisableTimer = 0;
    public bool attackEnabler = true;
    public float attackDisableTimer = 0;
    public bool damageEnabler = true;
    public float damageDisableTimer = 0;
    public bool sprintEnabler = true;
    public float sprintDisableTimer = 0;
    public bool walkSpeedMutiplyerEnabler = false;
    public float walkSpeedMutiplyerDisableTimer = 0;
    public bool healingEnabler = true;
    public float healingDisableTimer = 0;
    public bool onHit = false;
    public float onHitTimer = 0;



    public float WalkSpeed { get { return B_WalkSpeed * ((100 + E_WalkSpeed) / 100); } }
    public float MaxHealth { get { return B_MaxHealth + E_MaxHealth; } }
    public float Strength { get { return B_Strength + E_Strength; } }
    public float Defence { get { return B_Defence + E_Defence; } }
    public float CritRate { get { return B_CritRate + E_CritRate; } }
    public float CritDamage { get { return B_CritDamage + E_CritDamage; } }

    public float Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            //if (value > currentHealth) camEffect.SetTrigger("Heal");

            if (value < currentHealth) camEffect.SetTrigger("OnHit");

            currentHealth = value;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                currentCoinAmount = 0;

                //disable player object
                currentRb.bodyType = RigidbodyType2D.Static;
                behaviourEnabler = false;
                shopUI.SetActive(false);
                inventoryUI.SetActive(false);
                deathUI.SetActive(true);

                //drop item
                List<Lootings> dropList = new();


                for (int index = 0; index < inventoryData.Size; index++)
                {
                    if (UnityEngine.Random.Range(0, 100) >= 50)
                    {
                        dropList.Add(new Lootings(
                            inventoryData.GetItemAt(index).item,
                            100,
                            inventoryData.GetItemAt(index).quantity)
                        );
                        inventoryData.RemoveItem(index, -1);
                    }
                }

                ItemDropper ItemDropper = Instantiate(
                    itemDropper,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    Quaternion.identity,
                    GameObject.FindWithTag("Item").transform
                    ).GetComponent<ItemDropper>();
                ItemDropper.DropItems(dropList);


                //disable player object
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    child.gameObject.SetActive(false);
                }
                
            }
        }
    }

    public int CoinAmount { get { return currentCoinAmount; } set { currentCoinAmount = value; } }
    public List<Effection> GetEffectionList { get { return effectionList; } set { effectionList = value; } }
    public List<Key> GetKeyList { get { return keyList; } set { keyList = value; } }

    [Serializable]
    public class Effection
    {
        public EdibleItemSO effectingItem;
        public float effectingTime;
    }

    [Serializable]
    public class Key
    {
        public KeySO key;
        public int quantity;
    }



    public void OnSceneLoaded()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
    }

    private void Awake()
    {
        currentHealth = MaxHealth;
    }
    
    private void Update()
    {
        if(!behaviourEnabler) return;

        animator.SetBool("isHit", onHit);
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        spriteRenderer.flipX = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2 ? Input.GetAxis("Horizontal") < 0 : spriteRenderer.flipX;

        //update timer
        UpdatePlayerStates();
        UpdateTimer();
        UpdateEffectionList();
        UpdateKeyList();
        UpdateCurrentWeapon();

        //actions
        
        if (healingEnabler && currentHealth != MaxHealth) Heal();
        if (Input.anyKey && movementEnabler) Moving();
        if (Input.GetKeyDown(sprintKey) && sprintEnabler && movementEnabler) Sprint();
        if (Input.GetKeyDown(meleeWeaponKey)) weaponControl = weaponControl != 1 ? 1 : 0;
        if (Input.GetKeyDown(rangedWeaponKey)) weaponControl = weaponControl != 2 ? 2 : 0;
        if (Input.GetKeyDown(usePotionKey) && equipmentData.GetItemAt(5).item != null)
        {
            EdibleItemSO potion = equipmentData.GetItemAt(5).item as EdibleItemSO;
            SetEffection(potion);
            equipmentData.RemoveItem(5, 1);
        } 
        if (Input.GetKeyDown(backpackKey))
        {
            inventoryUI.SetActive(!inventoryUI.activeInHierarchy);
            Time.timeScale = inventoryUI.activeInHierarchy ? 0 : 1;
        }
        if (Input.GetKey(useWeaponKey) && attackEnabler)
        {
            if(currentWeapon != null)
            {
                attackDisableTimer += currentWeapon.attackCooldown;
                summonWeapon.Summon();
            }
        }
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    pauseUI.SetActive(true);
        //}
       
    }

    private void Moving()
    { 
        int walkSpeedMutiplyer = walkSpeedMutiplyerEnabler ? 3 : 1;

        Vector2 movement = new(
            Input.GetAxis("Horizontal") * WalkSpeed * walkSpeedMutiplyer,
            Input.GetAxis("Vertical") * WalkSpeed * walkSpeedMutiplyer
        );

        currentRb.velocity = movement;
    }

    private void Sprint()
    {
        sprintDisableTimer += 2f;
        walkSpeedMutiplyerDisableTimer += 0.2f;
        damageDisableTimer += 0.2f;
    }

    private void Heal()
    {
        float healValue = Mathf.Min(MaxHealth - currentHealth, MaxHealth * 0.05f);
        Health += healValue;
        DamageText.InstantiateDamageText(damageText, transform.position, healValue, "Heal");
        healingDisableTimer = 5;
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!damageEnabler || !behaviourEnabler) return;

        float localDamage = damage / (1 + (0.001f * Defence));
        Vector2 localKnockbackForce = knockbackForce / (1 + (0.001f * Defence));
        float localKnockbackTime = knockbackTime / (1f + (0.001f * Defence));

        //update heath
        Health -= localDamage;

        //knockback
        currentRb.velocity = localKnockbackForce;

        //play audio
        audioPlayer.PlayOneShot(hitSound);

        //instantiate damege text
        DamageText.InstantiateDamageText(damageText, transform.position, localDamage, "PlayerHit");

        //camera shake
        CameraController camera = GameObject.FindWithTag("MainCamera").GetComponentInParent<CameraController>();
        StartCoroutine(camera.Shake(0.1f, 0.2f));

        //set timer
        movementDisableTimer = movementDisableTimer < localKnockbackTime ? localKnockbackTime : movementDisableTimer;
        healingDisableTimer = 20;
    }





    private void UpdatePlayerStates()
    {
        //update player statistics
        string[] attributes = { "E_walkSpeed", "E_maxHealth", "E_strength", "E_defence", "E_critRate", "E_critDamage" };
        List<object> items = new() { equipmentData.GetItemAt(0).item, equipmentData.GetItemAt(1).item, equipmentData.GetItemAt(2).item, currentWeapon };
        float[] results = new float[attributes.Length];

        for (int j = 0; j < effectionList.Count; j++)
        {
            items.Add(effectionList[j].effectingItem);
        }

        int i = 0;
        foreach (var attribute in attributes)
        {
            results[i] = 0;

            for(int k = 0; k < items.Count; k++)
            {
                if (items[k] != null) results[i] += (float)items[k].GetType().GetField(attribute).GetValue(items[k]);
            }
            i++;
        }

        E_WalkSpeed = results[0];
        E_MaxHealth = results[1];
        E_Strength = results[2];
        E_Defence = results[3];
        E_CritRate = results[4];
        E_CritDamage = results[5];

        //check overhealing
        if (currentHealth > MaxHealth) currentHealth = MaxHealth;
    }

    private void UpdateTimer()
    {
        //update timer
        movementDisableTimer = Mathf.Max(0, movementDisableTimer - Time.deltaTime);
        attackDisableTimer = Mathf.Max(0, attackDisableTimer - Time.deltaTime);
        damageDisableTimer = Mathf.Max(0, damageDisableTimer - Time.deltaTime);
        sprintDisableTimer = Mathf.Max(0, sprintDisableTimer - Time.deltaTime);
        walkSpeedMutiplyerDisableTimer = Mathf.Max(0, walkSpeedMutiplyerDisableTimer - Time.deltaTime);
        healingDisableTimer = Mathf.Max(0, healingDisableTimer - Time.deltaTime);
        onHitTimer = Mathf.Max(0, onHitTimer - Time.deltaTime);

        movementEnabler = movementDisableTimer <= 0;
        attackEnabler = attackDisableTimer <= 0;
        damageEnabler = damageDisableTimer <= 0;
        sprintEnabler = sprintDisableTimer <= 0;
        walkSpeedMutiplyerEnabler = !(walkSpeedMutiplyerDisableTimer <= 0);
        healingEnabler = healingDisableTimer <= 0;
        onHit = !(onHitTimer <= 0);
    }

    public void UpdateKeyList()
    {
        //update key list
        int indexOfKeyList = -1;
        foreach (Key key in keyList)
        {
            if (key.quantity <= 0)
            {
                indexOfKeyList = keyList.IndexOf(key);
            }
        }
        keyList.Remove(indexOfKeyList != -1 ? keyList[indexOfKeyList] : null);
    }

    private void UpdateEffectionList()
    {
        //update effection list
        int indexOfEffectionList = -1;
        foreach (Effection effectingItem in effectionList)
        {
            effectingItem.effectingTime -= Time.deltaTime;
            if (effectingItem.effectingTime <= 0)
            {
                indexOfEffectionList = effectionList.IndexOf(effectingItem);
            }
        }
        effectionList.Remove(indexOfEffectionList != -1 ? effectionList[indexOfEffectionList] : null);
    }

    public WeaponSO UpdateCurrentWeapon()
    {
        //update current weapon
        switch (weaponControl)
        {
            case 0:
                currentWeapon = attackEnabler ? null : currentWeapon;
                break;
            case 1:
                currentWeapon = attackEnabler ? (WeaponSO)equipmentData.GetItemAt(3).item : currentWeapon;
                break;
            case 2:
                currentWeapon = attackEnabler ? (WeaponSO)equipmentData.GetItemAt(4).item : currentWeapon;
                break;
        }

        return currentWeapon;
    }





    public void SetEquipment(int index)
    {
        ItemSO item = inventoryData.GetItemAt(index).item;

        if (item is EquippableItemSO)
        {
            switch (((EquippableItemSO)item).equipmentType)
            {
                case EquippableItemSO.EquipmentType.armor:
                    inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 0));
                    break;
                case EquippableItemSO.EquipmentType.jewelry:
                    inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 1));
                    break;
                case EquippableItemSO.EquipmentType.book:
                    inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 2));
                    break;
            }
        }
        else if (item is MeleeWeaponSO)
        {
            inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 3));
        }
        else if (item is RangedWeaponSO)
        {
            inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 4));
        }
        else if (item is EdibleItemSO)
        {
            inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 5));
        }
    }

    public void UnEquipment(int index)
    {
        equipmentData.AddItemTo(inventoryData.AddItem(equipmentData.RemoveItem(index, -1)), index);
    }

    public void SetEffection(EdibleItemSO edibleItem)
    {
        int indexOfEffectionList = 0;
        bool isEffectionExist = false;

        foreach (Effection effectingItem in effectionList)
        {
            if (edibleItem.ID == effectingItem.effectingItem.ID)
            {
                indexOfEffectionList = effectionList.IndexOf(effectingItem);
                isEffectionExist = true;
            }
        }

        if (isEffectionExist)
        {
            effectionList[indexOfEffectionList].effectingTime = edibleItem.effectTime;
        }
        else
        {
            if (edibleItem.effectTime != 0)
            {
                effectionList.Add(new Effection { effectingItem = edibleItem, effectingTime = edibleItem.effectTime });
            }
        }

        if (edibleItem.E_heal != 0)
        {
            float healValue = Mathf.Min(MaxHealth - currentHealth, edibleItem.E_heal);
            Health += healValue;

            DamageText.InstantiateDamageText(damageText, transform.position, healValue, "Heal");

            camEffect.SetTrigger("Heal");
        }  
    }


    public void PlayAnimator(string animatorName)
    {
        switch (animatorName)
        {
            case "Warning":

                warningAnim.SetTrigger("Play");

                break;
        }
    }

    public void RevivePlayer()
    {
        currentHealth = MaxHealth;
        currentRb.bodyType = RigidbodyType2D.Dynamic;
        behaviourEnabler = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
        }
    }

    public void DropItem(InventorySO inventory ,int index)
    {
        ItemDropper ItemDropper = Instantiate(
            itemDropper,
            transform.position,
            Quaternion.identity,
            GameObject.FindWithTag("Item").transform
            ).GetComponent<ItemDropper>();

        ItemDropper.DropItem(inventory.RemoveItem(index, -1));
    }
}