using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private Gradient ShieldBarGradient;

    [Header("Object Reference")]
    [SerializeField] private EnemyBehavior boss;
    [SerializeField] private TMP_Text healthText, bossName;
    [SerializeField] private Slider healthBarSlider, shieldBarSlider;
    [SerializeField] private Image healthBarFill, shieldBarFill;
    [SerializeField] private GameObject shieldBar;

    void Update()
    {
        try
        {
            boss = GameObject.FindWithTag("Boss").GetComponent<EnemyBehavior>();
        }
        catch
        {

        }

        if(boss != null)
        {
            setVisiable(true);

            bossName.text = boss.enemy.Name;
            healthBarSlider.value = boss.Health / boss.enemy.health;
            healthText.text = $"{Mathf.RoundToInt(boss.Health)} / {boss.enemy.health}";
            healthBarFill.color = healthBarGradient.Evaluate(boss.Health / boss.enemy.health);

            shieldBar.SetActive(boss.enemy.haveShield);
            if (boss.enemy.haveShield)
            {
                shieldBarSlider.value = boss.ShieldHealth / boss.enemy.shieldHealth;
                shieldBarFill.color = ShieldBarGradient.Evaluate(boss.ShieldHealth / boss.enemy.shieldHealth);
            }
        }
        else
        {
            setVisiable(false);
        }
    }

    private void setVisiable(bool Visiable)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(Visiable);
        }
    }
}
