using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool canReopen;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("Object Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D BoxCollider2D;
    [SerializeField] private Interactable interactable;

    [Header("Dynamic Data")]
    [SerializeField] private bool isOpen;



    void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            interactable.enabled = false;
            BoxCollider2D.enabled = false;

            animator.SetTrigger("Open");

            audioPlayer.PlayOneShot(openSound);
        }
    }

    public void CloseDoor()
    {
        if (isOpen && canReopen)
        {
            isOpen = false;
            interactable.enabled = true;
            BoxCollider2D.enabled = true;

            animator.SetTrigger("Close");

            audioPlayer.PlayOneShot(closeSound);
        }
    }
}
