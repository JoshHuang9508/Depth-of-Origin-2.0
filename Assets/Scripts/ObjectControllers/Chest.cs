using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Settings")]
    public bool canReopen;

    [Header("Loottings")]
    [SerializeField] private List<Lootings> coins;
    [SerializeField] private List<Lootings> lootings;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;

    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] public Interactable interactable;

    // Flags
    private bool isOpen;

    public void OpenChest()
    {
        if (!isOpen)
        {
            isOpen = true;
            animator.SetTrigger("Open");
            AudioPlayer.PlaySound(openSound);
            ItemDropper.Drop(transform.position, lootings);
            ItemDropper.Drop(transform.position, coins);
        }
    }
    public void CloseChest()
    {
        if (isOpen && canReopen)
        {
            isOpen = false;
            animator.SetTrigger("Close");
        }
    }
    public void SetChestContent(List<Lootings> coins, List<Lootings> lootings)
    {
        this.coins = coins;
        this.lootings = lootings;
    }
    public void AddChestContent(Lootings coin, Lootings looting)
    {
        coins.Add(coin);
        lootings.Add(looting);
    }
    public void RemoveChestContent(Lootings coin, Lootings looting)
    {
        coins.Remove(coin);
        lootings.Remove(looting);
    }
    public void ClearChestContent()
    {
        coins.Clear();
        lootings.Clear();
    }
    public bool IsOpen()
    {
        return isOpen;
    }
}
