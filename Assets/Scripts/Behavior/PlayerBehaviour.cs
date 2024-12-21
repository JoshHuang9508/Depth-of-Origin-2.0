using System.Collections.Generic;
using UnityEngine;
using Inventory;
using UserInterface;
using System;
using System.Threading.Tasks;
using TMPro;

public class PlayerBehaviour : MonoBehaviour, IDamageable
{
    [Header("Status")]
    [SerializeField] private float health;
    [SerializeField] private int coins;

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
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip deadSound;

    [Header("UI")]
    [SerializeField] private Interface backpackUI;
    [SerializeField] private Interface shopUI;
    [SerializeField] private Interface keyUI;
    [SerializeField] private PauseMenu pauseUI;
    [SerializeField] private DeathMenu deathUI;
    [SerializeField] private Dialog dialogUI;
    [SerializeField] public CamEffect camEffect;
    [SerializeField] public TextMeshPro KeyHint;

    [Header("Animators")]
    [SerializeField] private Animator animator;
    [SerializeField] private Animator warningAnim;

    [Header("References")]
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private Rigidbody2D currentRb;

    // Status
    public float Health => Mathf.Max(0, health);
    public int Coins => coins;
    public float MaxHealth => maxHealth + maxHealth_e;
    public float WalkSpeed => walkSpeed * ((100 + walkSpeed_e) / 100);
    public float Strength => strength + strength_e;
    public float Defence => defence + defence_e;
    public float CritRate => critRate + critRate_e;
    public float CritDamage => critDamage + critDamage_e;

    // Effection
    public List<Effection> effectionList = new();
    private float maxHealth_e, walkSpeed_e, strength_e, defence_e, critRate_e, critDamage_e;

    // Weapon
    public int WeaponIndex => weapon == null ? 0 : weapon.weaponType == WeaponType.melee ? 1 : weapon.weaponType == WeaponType.range ? 2 : 0;
    private WeaponSO weapon;

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

    private void Start()
    {
        Revive();
    }

    private void Update()
    {
        // Maybe move this to another method
        animator.SetBool("isHit", !IsTimerEnd(TimerType.Hit));

        UpdateStates();
        UpdateTimer();
        UpdateEffectionList();

        if (!isActive) return;
        if (IsTimerEnd(TimerType.Move)) Move();
        if (IsTimerEnd(TimerType.Heal) && Health != MaxHealth) Heal(MaxHealth * 0.05f);
        if (IsTimerEnd(TimerType.Attack) && Input.GetKey(useWeaponKey) && weapon != null) Attack(weapon);
        if (IsTimerEnd(TimerType.Sprint) && Input.GetKeyDown(sprintKey)) Sprint();
        if (Input.GetKeyDown(meleeWeaponKey)) SetWeapon(weapon != equipmentData.GetItemAt(3).item ? 1 : 0);
        if (Input.GetKeyDown(rangedWeaponKey)) SetWeapon(weapon != equipmentData.GetItemAt(4).item ? 2 : 0);
        if (Input.GetKeyDown(usePotionKey) && equipmentData.GetItemAt(5).item != null)
        {
            SetEffection(equipmentData.GetItemAt(5).item as PotionSO);
            equipmentData.RemoveItem(5, 1);
        }
        if (Input.GetKeyDown(backpackKey)) ToggleBackpackUI();
        if (Input.GetKeyDown(pauseKey)) TogglePauseMenu();
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

        SetTimer(TimerType.Attack, weapon.attackSpeed);
    }
    private void Heal(float value)
    {
        health += Mathf.Min(value, MaxHealth - Health);
        camEffect.PlayCamEffect(CamEffect.CamEffectType.Heal);
        DamageTextGenerator.SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.Heal);
        AudioPlayer.PlaySound(healSound);
        SetTimer(TimerType.Heal, 20);
    }
    private void Damage(float value)
    {
        health -= Mathf.Min(value, 0);
        camEffect.PlayCamEffect(CamEffect.CamEffectType.Hit);
        DamageTextGenerator.SetDamageText(transform.position, value, DamageTextDisplay.DamageTextType.PlayerHit);
        AudioPlayer.PlaySound(hitSound);
        MainCamera.Shake(0.1f, 0.2f);
        SetTimer(TimerType.Heal, 20f);
        if (Health <= 0) Kill();
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

        Vector3 direction = new Vector3(X, Y).normalized;

        currentRb.MovePosition(transform.position + WalkSpeed * percentage * Time.deltaTime * direction);
    }
    private void Sprint()
    {
        currentRb.velocity *= 2;
        SetTimer(TimerType.Sprint, 1f);
        SetTimer(TimerType.WalkSpeedMutiply, 0.1f);
    }
    private void Kill()
    {
        ToggleDeathMenu(UIOption.open);
        enabled = false;
        isActive = false;
        // currentRb.bodyType = RigidbodyType2D.Static;
        // characterSprite.enabled = false;

        // Make lootings
        List<Lootings> Lootings = new();
        for (int index = 0; index < backpackData.Size; index++)
        {
            if (UnityEngine.Random.Range(0, 100) < 50) continue;

            int quantity = UnityEngine.Random.Range(0, backpackData.GetItemAt(index).quantity); // 0 ~ quantity
            InventorySlot inventory = backpackData.RemoveItem(index, quantity);
            Lootings.Add(new(inventory.item, inventory.quantity, 100));
        }

        ItemDropper.Drop(transform.position, Lootings);
        // ItemDropper.Drop(transform.position, Coins);
        AudioPlayer.PlaySound(deadSound);
        ModifyBalance(-Coins);
    }
    // Any way to make it private?
    public void Revive()
    {
        health = MaxHealth;
        enabled = true;
        isActive = true;
        // currentRb.bodyType = RigidbodyType2D.Dynamic;
        // characterSprite.enabled = true;
    }

    ////////////
    // Update //
    ////////////

    private void UpdateStates()
    {
        string[] attributes = { "E_maxHealth", "E_walkSpeed", "E_strength", "E_defence", "E_critRate", "E_critDamage" };
        List<object> items = new() { equipmentData.GetItemAt(0).item, equipmentData.GetItemAt(1).item, equipmentData.GetItemAt(2).item, weapon };
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
    }
    private void UpdateTimer()
    {
        List<TimerType> keys = new(timerList.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            TimerType timerType = keys[i];
            timerList[timerType] -= Time.deltaTime;
        }
    }
    private void UpdateEffectionList()
    {
        int indexOfEffectionList = -1;
        foreach (Effection effectingItem in effectionList)
        {
            effectingItem.effectingTime -= Time.deltaTime;
            indexOfEffectionList = effectingItem.effectingTime <= 0 ? effectionList.IndexOf(effectingItem) : -1;
        }
        effectionList.Remove(indexOfEffectionList != -1 ? effectionList[indexOfEffectionList] : null);
    }

    ////////////////
    // Properties //
    ////////////////

    public void TakeDamage(AttackerType attackerType, float damage, bool isCrit, Vector2 kbForce, float kbTime)
    {
        if (!IsTimerEnd(TimerType.Damage) || !isActive || attackerType != AttackerType.enemy) return;

        float trueDamage = damage / (1 + (0.001f * Defence));
        Vector2 trueKbForce = kbForce / (1 + (0.001f * Defence));
        float trueKbTime = kbTime / (1f + (0.001f * Defence));

        Damage(trueDamage);
        currentRb.velocity = trueKbForce;

        SetTimer(TimerType.Hit, trueKbTime);
        SetTimer(TimerType.Damage, trueKbTime);
        SetTimer(TimerType.Move, trueKbTime);
    }
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
                weapon = null;
                break;
            case 1:
                weapon = (WeaponSO)equipmentData.GetItemAt(3).item;
                break;
            case 2:
                weapon = (WeaponSO)equipmentData.GetItemAt(4).item;
                break;
        }
    }
    // Make it a Animator system
    public void PlayAnimator(string animatorName)
    {
        switch (animatorName)
        {
            case "Warning":
                warningAnim.SetTrigger("Play");
                break;
        }
    }
    public async Task PlayDialog(string nameText, string[] dialogTexts)
    {
        isActive = false;
        await dialogUI.ShowDialog(nameText, dialogTexts);
        isActive = true;
    }
    public void SetKeyHint(string key)
    {
        KeyHint.text = $"[{key}]";
    }
    public bool ModifyBalance(int value)
    {
        if (Coins + value < 0)
        {
            return false;
        }
        else
        {
            coins += value;
            return true;
        }
    }
    public void SetTimer(TimerType timerType, float value)
    {
        timerList[timerType] = Mathf.Max(value, timerList[timerType]);
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