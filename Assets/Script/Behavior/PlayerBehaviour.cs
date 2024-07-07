using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UserInterface;
using System;
using TMPro;
using System.Threading.Tasks;

public class PlayerBehaviour : MonoBehaviour, Damageable
{
    [Header("Stats")]
    [SerializeField] private float health;
    public int coinAmount = 0;
    public InventorySO inventoryData;
    public InventorySO equipmentData;
    public List<Key> keyList = new();
    public List<Effection> effectionList = new();

    [Header("Attributes Setting")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float strength;
    [SerializeField] private float defence;
    [SerializeField] private float critRate;
    [SerializeField] private float critDamage;

    [Header("Key Settings")]
    public KeyCode sprintKey;
    public KeyCode backpackKey;
    public KeyCode usePotionKey;
    public KeyCode useWeaponKey;
    public KeyCode meleeWeaponKey;
    public KeyCode rangedWeaponKey;
    public KeyCode interactKey;

    [Header("Weapon")]
    [SerializeField] private WeaponSO currentWeapon;

    [Header("Effection")]
    [SerializeField] private float maxHealth_e;
    [SerializeField] private float walkSpeed_e;
    [SerializeField] private float strength_e;
    [SerializeField] private float defence_e;
    [SerializeField] private float critRate_e;
    [SerializeField] private float critDamage_e;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip hitSound;

    [Header("Reference")]
    public GameObject inventoryUI;
    public GameObject shopUI;
    public GameObject pauseUI;
    public GameObject deathUI;
    public Animator cutscene;
    [SerializeField] private Animator camEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator warningAnim;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private EquipmentDisplay equipmentDisplay;
    [SerializeField] private GameObject damageTextReference;
    [SerializeField] private GameObject itemDropperReference;
    [SerializeField] private GameObject dialogObjectReference;

    //Runtime data
    private GameObject dialog;
    private TMP_Text dialogText;
    private Vector2 mousePos;
    private Vector2 currentPos;
    private Vector2 diraction;
    private float facingAngle;
    private float noMoveTimer = 0;
    private float noAttackTimer = 0;
    private float noDamageTimer = 0;
    private float noSprintTimer = 0;
    private float noWalkSpeedMutiplyTimer = 0;
    private float noHealTimer = 0;
    private float isHitTimer = 0;

    public float MaxHealth { get { return maxHealth + maxHealth_e; } }
    public float WalkSpeed { get { return walkSpeed * ((100 + walkSpeed_e) / 100); } }
    public float Strength { get { return strength + strength_e; } }
    public float Defence { get { return defence + defence_e; } }
    public float CritRate { get { return critRate + critRate_e; } }
    public float CritDamage { get { return critDamage + critDamage_e; } }

    public bool canActive { get; private set; }
    public bool canMove { get; private set; }
    public bool canAttack { get; private set; }
    public bool canBeDamaged { get; private set; }
    public bool canSprint { get; private set; }
    public bool canHeal { get; private set; }
    public bool isWalkSpeedMutiply { get; private set; }
    public bool isHit { get; private set; }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            //if (value > currentHealth) camEffect.SetTrigger("Heal");

            if (value < health) camEffect.SetTrigger("OnHit");

            health = Mathf.Max(0, value);

            if (health <= 0) KillPlayer();
        }
    }

    public void OnSceneLoaded()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
    }

    private void Start()
    {
        health = MaxHealth;
        canActive = true;

        dialog = Instantiate(
            dialogObjectReference,
            transform.position + new Vector3(0, 1.5f, 0),
            Quaternion.identity,
            transform);
        dialog.SetActive(false);
        dialogText = dialog.GetComponentInChildren<TMP_Text>();
    }
    
    private void Update()
    {
        animator.SetBool("isHit", isHit);

        //update timer
        UpdateStates();
        UpdateTimer();
        UpdateEffectionList();
        UpdateKeyList();
        UpdateWeapon();
        UpdateMousePos();

        //actions
        if (canActive)
        {
            if (canHeal && health != MaxHealth) Heal();
            if (canMove) Moving();
            if (canSprint && canMove && Input.GetKeyDown(sprintKey)) Sprint();
            if (Input.GetKeyDown(meleeWeaponKey) && canAttack) if (currentWeapon != equipmentData.GetItemAt(3).item) SetWeapon(1); else SetWeapon(0);
            if (Input.GetKeyDown(rangedWeaponKey) && canAttack) if (currentWeapon != equipmentData.GetItemAt(4).item) SetWeapon(2); else SetWeapon(0);
            if (Input.GetKeyDown(usePotionKey) && equipmentData.GetItemAt(5).item != null)
            {
                SetEffection(equipmentData.GetItemAt(5).item as PotionSO);
                equipmentData.RemoveItem(5, 1);
            }
            if (Input.GetKeyDown(backpackKey))
            {
                inventoryUI.SetActive(!inventoryUI.activeInHierarchy);
                //***
                Time.timeScale = inventoryUI.activeInHierarchy ? 0 : 1;
            }
            if (canAttack && Input.GetKey(useWeaponKey))
            {
                if (currentWeapon != null)
                {
                    noAttackTimer += currentWeapon.attackCooldown;
                    Attacking();
                }
            }
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    pauseUI.SetActive(true);
            //}
        }
    }




    /////////////
    //functions//
    /////////////

    private void Attacking()
    {
        WeaponSO weapon = currentWeapon;

        if (weapon == null) return;

        for (var i = weaponSprite.gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(weaponSprite.gameObject.transform.GetChild(i).gameObject);
        }

        if (weapon is MeleeWeaponSO)
        {
            MeleeWeaponSO meleeWeapon = weapon as MeleeWeaponSO;

            var sword = Instantiate(
                meleeWeapon.weaponObject,
                weaponSprite.gameObject.transform.position,
                Quaternion.identity,
                weaponSprite.gameObject.transform
                ).GetComponent<MeleeWeapon>();

            sword.target = Target.enemy;
            sword.weapon = meleeWeapon;
            sword.strength = Strength;
            sword.critRate = CritRate;
            sword.critDamage = CritDamage;
            sword.isflip = diraction.x < 0;

            weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle - 90);
        }
        else if (weapon is RangedWeaponSO)
        {
            RangedWeaponSO rangedWeapon = weapon as RangedWeaponSO;

            switch (rangedWeapon.shootingType)
            {
                case RangedWeaponSO.ShootingType.Single:

                    var projectile = Instantiate(
                        rangedWeapon.projectileObject,
                        weaponSprite.gameObject.transform.position,
                        Quaternion.Euler(0, 0, facingAngle - 90),
                        GameObject.FindWithTag("Item").transform
                        ).GetComponent<RangedWeapon>();

                    projectile.target = Target.enemy;
                    projectile.weapon = rangedWeapon;
                    projectile.strength = Strength;
                    projectile.critRate = CritRate;
                    projectile.critDamage = CritDamage;
                    projectile.startAngle = Quaternion.Euler(0, 0, facingAngle);
                    break;

                case RangedWeaponSO.ShootingType.Split:

                    for (int i = -60 + (120 / (rangedWeapon.splitAmount + 1)); i < 60; i += 120 / (rangedWeapon.splitAmount + 1))
                    {
                        var projectile_split = Instantiate(
                            rangedWeapon.projectileObject,
                            weaponSprite.gameObject.transform.position,
                            Quaternion.Euler(0, 0, facingAngle + i - 90),
                            GameObject.FindWithTag("Item").transform
                            ).GetComponent<RangedWeapon>(); ;

                        projectile_split.target = Target.enemy;
                        projectile_split.weapon = rangedWeapon;
                        projectile_split.strength = Strength;
                        projectile_split.critRate = CritRate;
                        projectile_split.critDamage = CritDamage;
                        projectile_split.startAngle = Quaternion.Euler(0, 0, facingAngle + i);
                    }
                    break;
            }
        }
    }

    private void Moving()
    {
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        characterSprite.flipX = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2 ? Input.GetAxis("Horizontal") < 0 : characterSprite.flipX;

        int walkSpeedMutiplyer = isWalkSpeedMutiply ? 3 : 1;

        Vector2 movement = new(
            Input.GetAxis("Horizontal") * WalkSpeed * walkSpeedMutiplyer,
            Input.GetAxis("Vertical") * WalkSpeed * walkSpeedMutiplyer
        );

        currentRb.velocity = movement;
    }

    private void Sprint()
    {
        noSprintTimer += 2f;
        noWalkSpeedMutiplyTimer = noWalkSpeedMutiplyTimer >= 0.2f ? noWalkSpeedMutiplyTimer : 0.2f;
        noDamageTimer = noDamageTimer >= 0.2f ? noDamageTimer : 0.2f;
    }

    private void Heal()
    {
        float healValue = Mathf.Min(MaxHealth - health, MaxHealth * 0.05f);
        Health += healValue;
        SetDamageText(transform.position, healValue, "Heal");
        noHealTimer = 5;
    }

    public void OnHit(float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!canBeDamaged || !canActive) return;

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
        SetDamageText(transform.position, localDamage, "PlayerHit");

        //camera shake
        MainCamera camera = GameObject.FindWithTag("MainCamera").GetComponentInParent<MainCamera>();
        StartCoroutine(camera.Shake(0.1f, 0.2f));

        //set timer
        noMoveTimer = noMoveTimer < localKnockbackTime ? localKnockbackTime : noMoveTimer;
        noHealTimer = 20;
        noDamageTimer += 0.2f;
    }




    //////////
    //Update//
    //////////

    private void UpdateStates()
    {
        //update player statistics
        string[] attributes = { "E_maxHealth", "E_walkSpeed", "E_strength", "E_defence", "E_critRate", "E_critDamage" };
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

        maxHealth_e = results[0];
        walkSpeed_e = results[1];
        strength_e = results[2];
        defence_e = results[3];
        critRate_e = results[4];
        critDamage_e = results[5];

        //check overhealing
        if (health > MaxHealth) health = MaxHealth;
    }

    private void UpdateTimer()
    {
        //update timer
        noMoveTimer = Mathf.Max(0, noMoveTimer - Time.deltaTime);
        noAttackTimer = Mathf.Max(0, noAttackTimer - Time.deltaTime);
        noDamageTimer = Mathf.Max(0, noDamageTimer - Time.deltaTime);
        noSprintTimer = Mathf.Max(0, noSprintTimer - Time.deltaTime);
        noWalkSpeedMutiplyTimer = Mathf.Max(0, noWalkSpeedMutiplyTimer - Time.deltaTime);
        noHealTimer = Mathf.Max(0, noHealTimer - Time.deltaTime);
        isHitTimer = Mathf.Max(0, isHitTimer - Time.deltaTime);

        canMove = noMoveTimer <= 0;
        canAttack = noAttackTimer <= 0;
        canBeDamaged = noDamageTimer <= 0;
        canSprint = noSprintTimer <= 0;
        isWalkSpeedMutiply = !(noWalkSpeedMutiplyTimer <= 0);
        canHeal = noHealTimer <= 0;
        isHit = !(isHitTimer <= 0);
    }

    private void UpdateKeyList()
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

    private void UpdateWeapon()
    {
        //update current weapon
        if (currentWeapon == null)
        {
            weaponSprite.sprite = null;
            weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle);
        }
        else
        {
            if (currentWeapon is MeleeWeaponSO) weaponSprite.sprite = null;
            else if (currentWeapon is RangedWeaponSO)
            {
                weaponSprite.sprite = currentWeapon.Image;
                weaponSprite.gameObject.transform.rotation = Quaternion.Euler(0, 0, facingAngle);
            }
        }
    }

    private void UpdateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos = transform.position;
        diraction = (mousePos - currentPos).normalized;
        facingAngle = Mathf.Atan2(diraction.y, diraction.x) * Mathf.Rad2Deg;
    }



    
    ////////////////
    //Modification//
    ////////////////

    public void SetEquipment(int index)
    {
        ItemSO item = inventoryData.GetItemAt(index).item;

        if (item is EquipmentSO)
        {
            switch (((EquipmentSO)item).equipmentType)
            {
                case EquipmentSO.EquipmentType.armor:
                    inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 0));
                    break;
                case EquipmentSO.EquipmentType.jewelry:
                    inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 1));
                    break;
                case EquipmentSO.EquipmentType.book:
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
        else if (item is PotionSO)
        {
            inventoryData.AddItem(equipmentData.AddItemTo(inventoryData.RemoveItem(index, -1), 5));
        }
    }

    public void UnEquipment(int index)
    {
        equipmentData.AddItemTo(inventoryData.AddItem(equipmentData.RemoveItem(index, -1)), index);
    }

    public void SetEffection(PotionSO edibleItem)
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
            float healValue = Mathf.Min(MaxHealth - health, edibleItem.E_heal);
            Health += healValue;

            SetDamageText(transform.position, healValue, "Heal");

            camEffect.SetTrigger("Heal");
        }  
    }

    public void SetWeapon(int index)
    {
        switch (index)
        {
            case 0:
                currentWeapon = null;
                break;
            case 1:
                currentWeapon = (WeaponSO)equipmentData.GetItemAt(3).item;
                break;
            case 2:
                currentWeapon = (WeaponSO)equipmentData.GetItemAt(4).item;
                break;
        }

        equipmentDisplay.SetEquipmentDisplay(index);
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

    public void KillPlayer()
    {
        //close UI
        shopUI.SetActive(false);
        inventoryUI.SetActive(false);
        deathUI.SetActive(true);

        //disable player object
        currentRb.bodyType = RigidbodyType2D.Static;
        characterSprite.enabled = false;
        canActive = false;

        //drop item
        List<int> dropItemIndexList = new();
        for (int index = 0; index < inventoryData.Size; index++)
        {
            if (UnityEngine.Random.Range(0, 100) >= 50)
            {
                dropItemIndexList.Add(index);
            }
        }
        foreach(int index in dropItemIndexList)
        {
            DropItem(inventoryData, index, -1);
        }

        coinAmount = 0;
    }

    public void RevivePlayer()
    {
        //enable player object
        health = MaxHealth;
        currentRb.bodyType = RigidbodyType2D.Dynamic;
        characterSprite.enabled = true;
        canActive = true;
    }

    public void DropItem(InventorySO inventory ,int index, int quantity)
    {
        ItemDropper ItemDropper = Instantiate(
            itemDropperReference,
            transform.position,
            Quaternion.identity,
            GameObject.FindWithTag("Item").transform
            ).GetComponent<ItemDropper>();
        ItemDropper.DropItem(inventory.RemoveItem(index, quantity));
    }

    public void SetActive(bool value)
    {
        canActive = value;
    }

    public async Task SetDialog(string[] dialogLines)
    {
        SetActive(false);
        dialog.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        dialog.SetActive(true);
       
        foreach(string line in dialogLines)
        {
            dialogText.text = line;
            await Task.Delay(1000);
            await WaitForClick();
        }

        SetActive(true);
        dialogText.text = "";
        dialog.SetActive(false);
    }


    private async Task WaitForClick()
    {
        while (!Input.GetKey(KeyCode.Mouse0))
        {
            await Task.Yield();
        }
    }

    public void SetDamageText(Vector3 position, float value, string type)
    {
        var damageText = Instantiate(
            damageTextReference,
            Camera.main.WorldToScreenPoint(position),
            Quaternion.identity,
            GameObject.Find("Displays").transform
            ).GetComponent<DamageTextDisplay>();

        damageText.SetDisplay(value, type);
    }
}

[Serializable]
public class Effection
{
    public PotionSO effectingItem;
    public float effectingTime;
}

[Serializable]
public class Key
{
    public KeySO key;
    public int quantity;
}