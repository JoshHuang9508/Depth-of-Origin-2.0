using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovementMelee : MonoBehaviour
{
    [Header("Object Reference")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Data")]
    public MeleeWeaponSO weaponData;
    public PlayerBehaviour playerData;

    [Header("Audio")]
    public AudioSource audioPlayer;
    public AudioClip swingSound;

    [Header("Stats")]
    public bool isflip;
}
