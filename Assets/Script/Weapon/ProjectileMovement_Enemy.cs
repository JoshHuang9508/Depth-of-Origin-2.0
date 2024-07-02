using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectileMovement_Enemy : MonoBehaviour
{
    [Header("Object Reference")]
    public Rigidbody2D objectRigidbody;
    public SpriteRenderer spriteRenderer;
    public Collider2D thisCollider;
    public TrailRenderer trail;

    [Header("Setting")]
    [SerializeField] private bool ignoreWalls;

    [Header("Data")]
    public RangedWeaponSO rangedWeapon;
    public EnemySO enemyData;

    [Header("Audio")]
    public AudioSource audioPlayer;
    public AudioClip shotSound;

    [Header("Stats")]
    public Quaternion startAngle;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageableObject = collision.GetComponentInParent<Damageable>();

        if (damageableObject != null)
        {
            if (collision.CompareTag("PlayerHitBox"))
            {
                Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;
                Vector2 direction = (Vector2)(collision.gameObject.transform.position - parentPos).normalized;

                damageableObject.OnHit(
                    enemyData.attackDamage,
                    false,
                    direction * enemyData.knockbackForce,
                    enemyData.knockbackTime);
            }
        }

        if ((collision.CompareTag("Wall") || collision.CompareTag("BreakableObject") || collision.CompareTag("Object") || collision.CompareTag("PlayerHitBox")) && !ignoreWalls)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ProjectileFly(startAngle);
        StartCoroutine(DestroyCooldown());
    }

    public void ProjectileFly(Quaternion angle)
    {
        Vector3 angleVec3 = angle.eulerAngles;
        objectRigidbody.velocity = new Vector3(
            enemyData.projectileFlySpeed * Mathf.Cos(angleVec3.z * Mathf.Deg2Rad),
            enemyData.projectileFlySpeed * Mathf.Sin(angleVec3.z * Mathf.Deg2Rad),
            0);
    }

    private IEnumerator DestroyCooldown()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

}
