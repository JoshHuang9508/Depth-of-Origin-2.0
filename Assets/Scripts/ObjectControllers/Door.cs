using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    public bool canReopen;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D BoxCollider2D;

    // Flags
    private bool isOpen;

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            BoxCollider2D.enabled = false;
            animator.SetTrigger("Open");
            AudioPlayer.PlaySound(openSound);
        }
    }

    public void CloseDoor()
    {
        if (isOpen && canReopen)
        {
            isOpen = false;
            BoxCollider2D.enabled = true;
            animator.SetTrigger("Close");
            AudioPlayer.PlaySound(closeSound);
        }
    }
}
