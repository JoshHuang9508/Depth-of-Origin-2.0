using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : RangedWeapon
{
    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        ProjectileFly(startAngle);
        Destroy(gameObject, 10f);
    }

    private void ProjectileFly(Quaternion angle)
    {
        audioPlayer.PlayOneShot(shotSound);

        Vector3 angleVec3 = angle.eulerAngles;
        objectRigidbody.velocity = new Vector3(
            weapon.flySpeed * Mathf.Cos(angleVec3.z * Mathf.Deg2Rad),
            weapon.flySpeed * Mathf.Sin(angleVec3.z * Mathf.Deg2Rad),
            0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageableObject = collision.GetComponentInParent<Damageable>();

        if (damageableObject != null)
        {
            switch (target)
            {
                case Target.enemy:
                    if (collision.CompareTag("HitBox") || collision.CompareTag("BreakableObject"))
                    {
                        Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;
                        Vector2 direction = (collision.gameObject.transform.position - parentPos).normalized;

                        bool isCrit = Random.Range(0f, 100f) <= critRate;

                        damageableObject.OnHit(
                            weapon.weaponDamage * (1 + (0.01f * strength)) * (isCrit ? 1 + (0.01f * critDamage) : 1),
                            isCrit,
                            direction * weapon.knockbackForce,
                            weapon.knockbackTime);

                        Destroy(gameObject);
                    }
                    break;

                case Target.player:
                    if (collision.CompareTag("PlayerHitBox"))
                    {
                        Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;
                        Vector2 direction = (collision.gameObject.transform.position - parentPos).normalized;

                        bool isCrit = Random.Range(0f, 100f) <= critRate;

                        damageableObject.OnHit(
                            weapon.weaponDamage * (1 + (0.01f * strength)) * (isCrit ? 1 + (0.01f * critDamage) : 1),
                            isCrit,
                            direction * weapon.knockbackForce,
                            weapon.knockbackTime);

                        Destroy(gameObject);
                    }
                    break;
            }
        }

        if (collision.CompareTag("Wall") || collision.CompareTag("Shield") || collision.CompareTag("Object"))
        {
            Destroy(gameObject);
        }
    }
}
