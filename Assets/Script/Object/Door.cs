using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Attributes")]
    public bool isOpen;
    public bool canReopen;

    [Header("Audios")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D BoxCollider2D;

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
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
            BoxCollider2D.enabled = true;

            animator.SetTrigger("Close");

            audioPlayer.PlayOneShot(closeSound);
        }
    }
}
