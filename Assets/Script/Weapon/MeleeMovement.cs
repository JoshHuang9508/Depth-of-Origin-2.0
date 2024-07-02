using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMovement : WeaponMovementMelee
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageableObject = collision.GetComponentInParent<Damageable>();

        if (damageableObject != null)
        {
            if (collision.CompareTag("HitBox") || collision.CompareTag("BreakableObject"))
            {
                Vector3 parentPos = gameObject.GetComponentInParent<Transform>().position;
                Vector2 direction = (collision.gameObject.transform.position - parentPos).normalized;

                bool isCrit = Random.Range(0f, 100f) <= playerData.CritRate;

                damageableObject.OnHit(
                    weaponData.weaponDamage * (1 + (0.01f * playerData.Strength)) * (isCrit ? 1 + (0.01f * playerData.CritDamage) : 1),
                    isCrit,
                    direction * weaponData.knockbackForce,
                    weaponData.knockbackTime);

                //camera shake
                CameraController camera = GameObject.FindWithTag("MainCamera").GetComponentInParent<CameraController>();
                StartCoroutine(camera.Shake(0.1f, 0.2f));
            }
        }
    }

    private void Start()
    {
        audioPlayer = GameObject.FindWithTag("AudioPlayer").GetComponent<AudioSource>();

        WeaponSwing(isflip);
    }

    public void WeaponSwing(bool _isflip)
    {
        audioPlayer.PlayOneShot(swingSound);

        spriteRenderer.flipX = _isflip;
        spriteRenderer.transform.Rotate(Vector3.forward, 90f);

        animator.speed = weaponData.attackSpeed;
        animator.SetBool("isflip", _isflip);
        animator.SetTrigger("swing");
    }
}
