using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UserInterface;
using System;
using TMPro;
using System.Threading.Tasks;
using NUnit.Framework;

public class PlayerBehaviour : MonoBehaviour, IDamageable
{
    [Header("Status")]
    public List<Key> keyList = new();

    [Header("Datas")]
    public InventorySO backpackData;
    public InventorySO equipmentData;
    public InventorySO keyData;
    public InventorySO shopData;

    [Header("KeyCode")]
    public KeyCode sprintKey;
    public KeyCode backpackKey;
    public KeyCode usePotionKey;
    public KeyCode useWeaponKey;
    public KeyCode meleeWeaponKey;
    public KeyCode rangedWeaponKey;
    public KeyCode interactKey;
    public KeyCode pauseKey;

    [Header("Attributes")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float strength;
    [SerializeField] private float defence;
    [SerializeField] private float critRate;
    [SerializeField] private float critDamage;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    [Header("UI")]
    [SerializeField] private Interface backpackUI;
    [SerializeField] private Interface shopUI;
    [SerializeField] private PauseMenu pauseUI;
    [SerializeField] private DeathMenu deathUI;
    [SerializeField] public CamEffect camEffect;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator warningAnim;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Rigidbody2D currentRb;
    [SerializeField] private EquipmentDisplay equipmentDisplay;
    [SerializeField] private GameObject damageTextReference;
    [SerializeField] private GameObject itemDropperReference;
    [SerializeField] private GameObject dialogObjectReference;

    // Status
    public float health { get; private set; }
    public int coins { get; private set; }

    // Effection
    public List<Effection> effectionList = new();
    private float maxHealth_e, walkSpeed_e, strength_e, defence_e, critRate_e, critDamage_e;

    // Weapon
    private WeaponSO currentWeapon;

    // Flags
    public bool isActive, isInterfaceOpen;

    // Timer
    public enum TimerType { Move, Attack, Damage, Sprint, Heal, WalkSpeedMutiply, Hit }
    private Dictionary<TimerType, float> timerList = new()
    {
        { TimerType.Move, 0 },
        { TimerType.Attack, 0 },
        { TimerType.Damage, 0 },
        { TimerType.Sprint, 0 },
        { TimerType.Heal, 0 },
        { TimerType.WalkSpeedMutiply, 0 },
        { TimerType.Hit, 0 }
    };

    // Timer Getter
    public float MaxHealth { get { return maxHealth + maxHealth_e; } }
    public float WalkSpeed { get { return walkSpeed * ((100 + walkSpeed_e) / 100); } }
    public float Strength { get { return strength + strength_e; } }
    public float Defence { get { return defence + defence_e; } }
    public float CritRate { get { return critRate + critRate_e; } }
    public float CritDamage { get { return critDamage + critDamage_e; } }

    // Runtime data
    private GameObject dialog;
    private TMP_Text dialogText;

    // Direction
    private Vector2 mousePos, currentPos, diraction;
    private float facingAngle;

    private void Start()
    {
        health = MaxHealth;
        isActive = true;

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
        animator.SetBool("isHit", IsTimerEnd(TimerType.Hit));

        //update
        UpdateStates();
        UpdateTimer();
        UpdateEffectionList();
        // UpdateKeyList();
        // UpdateWeapon();
        UpdateMousePos();

        //actions
        if (isActive)
        {
            if (IsTimerEnd(TimerType.Heal) && health != MaxHealth)
            {
                SetTimer(TimerType.Heal, 0.5f);
                Heal(MaxHealth * 0.05f);
            };
            if (IsTimerEnd(TimerType.Attack) && Input.GetKey(useWeaponKey) && currentWeapon != null)
            {
                SetTimer(TimerType.Attack, currentWeapon.attackSpeed);
                Attack(currentWeapon);
            }
            if (IsTimerEnd(TimerType.Move)) Move();
            if (IsTimerEnd(TimerType.Sprint) && Input.GetKeyDown(sprintKey)) Sprint();
            if (Input.GetKeyDown(meleeWeaponKey)) if (currentWeapon != equipmentData.GetItemAt(3).item) SetWeapon(1); else SetWeapon(0);
            if (Input.GetKeyDown(rangedWeaponKey)) if (currentWeapon != equipmentData.GetItemAt(4).item) SetWeapon(2); else SetWeapon(0);
            if (Input.GetKeyDown(usePotionKey) && equipmentData.GetItemAt(5).item != null)
            {
                SetEffection(equipmentData.GetItemAt(5).item as PotionSO);
                equipmentData.RemoveItem(5, 1);
            }
            if (Input.GetKeyDown(backpackKey)) ToggleBackpackUI();
            if (Input.GetKeyDown(pauseKey)) TogglePauseMenu();
        }
    }

    ////////
    // UI //
    ////////

    public enum UIOption
    {
        toggle, open, close
    }
    public void ToggleBackpackUI(UIOption option = UIOption.toggle)
    {
        if (pauseUI.IsActive || deathUI.IsActive) return;

        bool canInteract = !backpackUI.IsActive;

        switch (option)
        {
            case UIOption.toggle:
                CloseAllUI();
                if (canInteract) backpackUI.Toggle();
                break;
            case UIOption.open:
                CloseAllUI();
                backpackUI.Open();
                break;
            case UIOption.close:
                backpackUI.Close();
                break;
        }
    }
    public void ToggleShopUI(UIOption option = UIOption.toggle)
    {
        if (pauseUI.IsActive || deathUI.IsActive) return;

        bool canInteract = !shopUI.IsActive;

        switch (option)
        {
            case UIOption.toggle:
                CloseAllUI();
                if (canInteract) shopUI.Toggle();
                break;
            case UIOption.open:
                CloseAllUI();
                shopUI.Open();
                break;
            case UIOption.close:
                shopUI.Close();
                break;
        }
    }
    public void TogglePauseMenu(UIOption option = UIOption.toggle)
    {
        if (deathUI.IsActive) return;

        bool canInteract = !shopUI.IsActive && !backpackUI.IsActive;

        switch (option)
        {
            case UIOption.toggle:
                CloseAllUI();
                if (canInteract) pauseUI.Toggle();
                break;
            case UIOption.open:
                CloseAllUI();
                pauseUI.Open();
                break;
            case UIOption.close:
                pauseUI.Close();
                break;
        }
    }
    public void ToggleDeathMenu(UIOption option = UIOption.toggle)
    {
        bool canInteract = !deathUI.IsActive;

        switch (option)
        {
            case UIOption.toggle:
                CloseAllUI();
                if (canInteract) deathUI.Toggle();
                break;
            case UIOption.open:
                CloseAllUI();
                deathUI.Open();
                break;
            case UIOption.close:
                deathUI.Close();
                break;
        }

    }
    private void CloseAllUI()
    {
        backpackUI.Close();
        shopUI.Close();
    }

    ///////////////
    // Abilities //
    ///////////////

    private void Attack(WeaponSO weapon = null)
    {
        if (weapon == null) return;

        switch (weapon.weaponType)
        {
            case WeaponType.melee:
                // MeleeAttack(weapon);
                break;
            case WeaponType.range:
                // RangedAttack(weapon);
                break;
        }
    }
    private void Heal(float value)
    {
        health += Mathf.Min(value, MaxHealth - health);
        SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.Heal);
        camEffect.PlayCamEffect(CamEffect.CamEffectType.Heal);
        // AudioPlayer.Playsound(healSound);
    }
    private void Damage(float value)
    {
        health -= value;
        SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        camEffect.PlayCamEffect(CamEffect.CamEffectType.Hit);
        // AudioPlayer.Playsound(hitSound);
        _ = MainCamera.Shake(0.1f, 0.2f);
    }
    private void Move()
    {
        animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        characterSprite.flipX = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2 ? Input.GetAxis("Horizontal") < 0 : characterSprite.flipX;

        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");
        float csc = 1 / Mathf.Sin(Mathf.Atan2(Y, X));
        float sec = 1 / Mathf.Cos(Mathf.Atan2(Y, X));
        float maxStrenthRaw = Mathf.Abs(csc) < Mathf.Abs(sec) ? Mathf.Abs(csc) : Mathf.Abs(sec);
        float inputStrenthRaw = Mathf.Sqrt(X * X + Y * Y);
        float percentage = inputStrenthRaw / maxStrenthRaw;

        Vector2 movement = WalkSpeed * percentage * new Vector2(X, Y).normalized;

        currentRb.velocity = movement;
    }
    private void Sprint()
    {
        currentRb.velocity *= 2;
        SetTimer(TimerType.Sprint, 1f);
        SetTimer(TimerType.WalkSpeedMutiply, 0.1f);
    }
    public void Damage(AttackerType attackerType, float damage, bool isCrit, Vector2 knockbackForce, float knockbackTime)
    {
        if (!IsTimerEnd(TimerType.Damage) || !isActive || attackerType != AttackerType.enemy) return;

        float trueDamage = damage / (1 + (0.001f * Defence));
        Vector2 trueKnockbackForce = knockbackForce / (1 + (0.001f * Defence));
        float trueKnockbackTime = knockbackTime / (1f + (0.001f * Defence));

        Damage(trueDamage);
        currentRb.velocity = trueKnockbackForce;

        SetTimer(TimerType.Hit, 0.1f);
        SetTimer(TimerType.Damage, 0.2f);
        SetTimer(TimerType.Move, trueKnockbackTime);
        SetTimer(TimerType.Heal, 20f);
    }

    ////////////
    // Update //
    ////////////

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

            for (int k = 0; k < items.Count; k++)
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
        foreach (TimerType timerType in timerList.Keys)
        {
            timerList[timerType] -= Time.deltaTime;
        }
    }
    // private void UpdateKeyList()
    // {
    //     //update key list
    //     int indexOfKeyList = -1;
    //     foreach (Key key in keyList)
    //     {
    //         if (key.quantity <= 0)
    //         {
    //             indexOfKeyList = keyList.IndexOf(key);
    //         }
    //     }
    //     keyList.Remove(indexOfKeyList != -1 ? keyList[indexOfKeyList] : null);
    // }
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
    private void UpdateMousePos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos = transform.position;
        diraction = (mousePos - currentPos).normalized;
        facingAngle = Mathf.Atan2(diraction.y, diraction.x) * Mathf.Rad2Deg;
    }

    ////////////////
    // Properties //
    ////////////////

    public void SetEquipment(int index)
    {
        ItemSO item = backpackData.GetItemAt(index).item;

        if (item is EquipmentSO equipment)
        {
            switch (equipment.equipmentType)
            {
                case EquipmentType.armor:
                    backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 0));
                    break;
                case EquipmentType.jewelry:
                    backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 1));
                    break;
                case EquipmentType.book:
                    backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 2));
                    break;
            }
        }
        else if (item is WeaponSO weapon)
        {
            switch (weapon.weaponType)
            {
                case WeaponType.melee:
                    backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 3));
                    break;
                case WeaponType.range:
                    backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 4));
                    break;
            }
        }
        else if (item is PotionSO)
        {
            backpackData.AddItem(equipmentData.AddItemTo(backpackData.RemoveItem(index, -1), 5));
        }
    }
    public void UnEquipment(int index)
    {
        equipmentData.AddItemTo(backpackData.AddItem(equipmentData.RemoveItem(index, -1)), index);
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

        if (edibleItem.E_heal != 0) Heal(edibleItem.E_heal);
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
        ToggleDeathMenu(UIOption.open);

        //disable player object
        currentRb.bodyType = RigidbodyType2D.Static;
        characterSprite.enabled = false;
        isActive = false;

        //drop item
        List<int> dropItemIndexList = new();
        for (int index = 0; index < backpackData.Size; index++)
        {
            if (UnityEngine.Random.Range(0, 100) >= 50)
            {
                dropItemIndexList.Add(index);
            }
        }
        foreach (int index in dropItemIndexList)
        {
            DropItem(backpackData, index, -1);
        }
        ModifyCoin(-coins);
    }
    public void RevivePlayer()
    {
        //enable player object
        health = MaxHealth;
        currentRb.bodyType = RigidbodyType2D.Dynamic;
        characterSprite.enabled = true;
        isActive = true;
    }
    public void DropItem(InventorySO inventory, int index, int quantity)
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
        isActive = value;
    }
    public async Task SetDialog(string[] dialogLines)
    {
        SetActive(false);
        dialog.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        dialog.SetActive(true);

        foreach (string line in dialogLines)
        {
            dialogText.text = line;
            await Task.Delay(1000);

            // Wait for click
            while (!Input.GetKey(KeyCode.Mouse0))
            {
                await Task.Yield();
            }
        }

        SetActive(true);
        dialogText.text = "";
        dialog.SetActive(false);
    }
    public void SetDamageText(Vector3 position, float value, DamageTextDisplay.DamageTextType type)
    {
        var damageText = Instantiate(
            damageTextReference,
            Camera.main.WorldToScreenPoint(position),
            Quaternion.identity,
            GameObject.Find("Displays").transform
            ).GetComponent<DamageTextDisplay>();

        damageText.SetDisplay(value, type);
    }
    public void ModifyCoin(int value)
    {
        coins += value;
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