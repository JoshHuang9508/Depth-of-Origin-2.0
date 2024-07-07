using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public MeleeWeaponSO weapon;

    [Header("Attributes")]
    public float strength;
    public float critRate;
    public float critDamage;
    public bool isflip;
    public Target target;

    [Header("Reference")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioPlayer;
    public AudioClip swingSound;
}

public enum Target
{
    player, enemy
}
