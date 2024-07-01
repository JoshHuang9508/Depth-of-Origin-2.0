using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectileMovement_Player : WeaponMovementRanged
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageableObject = collision.GetComponentInParent<Damageable>();

        if (damageableObject != null)
        {
            if (collision.CompareTag("HitBox") || collision.CompareTag("BreakableObject"))
            {
                Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;
                Vector2 direction = (Vector2)(collision.gameObject.transform.position - parentPos).normalized;

                bool isCrit = Random.Range(0f, 100f) <= playerData.CritRate;

                damageableObject.OnHit(
                    weaponData.weaponDamage * (1 + (0.01f * playerData.Strength)) * (isCrit ? 1 + (0.01f * playerData.CritDamage) : 1),
                    isCrit,
                    direction * weaponData.knockbackForce,
                    weaponData.knockbackTime);

                Destroy(gameObject);
            }
        }

        if (collision.CompareTag("Wall") || collision.CompareTag("Shield") || collision.CompareTag("Object"))
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        ProjectileFly(startAngle);
        StartCoroutine(DestroyCooldown());
    }

    public void ProjectileFly(Quaternion angle)
    {
        audioPlayer.PlayOneShot(shotSound);

        Vector3 angleVec3 = angle.eulerAngles;
        objectRigidbody.velocity = new Vector3(
            weaponData.flySpeed * Mathf.Cos(angleVec3.z * Mathf.Deg2Rad),
            weaponData.flySpeed * Mathf.Sin(angleVec3.z * Mathf.Deg2Rad),
            0);
    }

    private IEnumerator DestroyCooldown()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
