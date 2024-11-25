using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RangedWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public RangedWeaponSO weapon;

    [Header("Attributes")]
    public float strength;
    public float critRate;
    public float critDamage;
    public Quaternion startAngle;
    public bool isflip;
    public Target target;

    [Header("References")]
    public Rigidbody2D objectRigidbody;
    public SpriteRenderer spriteRenderer;
    public Collider2D thisCollider;
    public TrailRenderer trail;

    [Header("Audios")]
    public AudioSource audioPlayer;
    public AudioClip shotSound;
}
