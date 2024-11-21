using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarDisplay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Gradient healthBarGradient;
    [SerializeField] private Gradient ShieldBarGradient;
    
    [Header("References")]
    [SerializeField] private TMP_Text healthText, bossName;
    [SerializeField] private Slider healthBarSlider, shieldBarSlider;
    [SerializeField] private Image healthBarFill, shieldBarFill;
    [SerializeField] private GameObject shieldBar;
    [SerializeField] private PlayerBehaviour player;

    private EnemyBehavior boss;

    void Update()
    {
        try
        {
            boss = GameObject.FindWithTag("Boss").GetComponent<EnemyBehavior>();
        }
        catch
        {
            //Debug.Log("No boss detected.");
        }

        if (boss != null && Vector3.Distance(player.transform.position, boss.transform.position) <= 15)
        {
            SetVisiable(true);
            SetDisplay();
        }
        else
        {
            SetVisiable(false);
        }
    }

    private void SetVisiable(bool Visiable)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(Visiable);
        }
    }

    private void SetDisplay()
    {
        bossName.text = boss.enemy.Name;
        healthBarSlider.value = boss.Health / boss.enemy.maxHealth;
        healthText.text = $"{Mathf.RoundToInt(boss.Health)} / {boss.enemy.maxHealth}";
        healthBarFill.color = healthBarGradient.Evaluate(boss.Health / boss.enemy.maxHealth);

        shieldBar.SetActive(boss.enemy.haveShield);
        if (boss.enemy.haveShield)
        {
            shieldBarSlider.value = boss.ShieldHealth / boss.enemy.maxShieldHealth;
            shieldBarFill.color = ShieldBarGradient.Evaluate(boss.ShieldHealth / boss.enemy.maxShieldHealth);
        }
    }
}
