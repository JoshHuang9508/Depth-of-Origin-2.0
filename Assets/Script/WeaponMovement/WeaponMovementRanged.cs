using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeaponMovementRanged : MonoBehaviour
{
    [Header("Object Reference")]
    public Rigidbody2D objectRigidbody;
    public SpriteRenderer spriteRenderer;
    public Collider2D thisCollider;
    public TrailRenderer trail;

    [Header("Data")]
    public RangedWeaponSO weaponData;
    public PlayerBehaviour playerData;

    [Header("Audio")]
    public AudioSource audioPlayer;
    public AudioClip shotSound;

    [Header("Stats")]
    public Quaternion startAngle;
}
