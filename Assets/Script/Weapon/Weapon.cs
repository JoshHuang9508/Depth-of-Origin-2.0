using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MeleeWeapon
{
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

        animator.speed = weapon.attackSpeed;
        animator.SetBool("isflip", _isflip);
        animator.SetTrigger("swing");
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

                        //camera shake
                        MainCamera camera = GameObject.FindWithTag("MainCamera").GetComponentInParent<MainCamera>();
                        StartCoroutine(camera.Shake(0.1f, 0.2f));
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

                        //camera shake
                        MainCamera camera = GameObject.FindWithTag("MainCamera").GetComponentInParent<MainCamera>();
                        StartCoroutine(camera.Shake(0.1f, 0.2f));
                    }
                    break;
            }
        }
    }
}
