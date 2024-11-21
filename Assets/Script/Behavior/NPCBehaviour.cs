using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCBehaviour : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D currentRb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    //Runtime data
    private PlayerBehaviour player;
}
