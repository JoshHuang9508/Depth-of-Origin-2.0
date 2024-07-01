using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BossSummonerContorller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int neededActionTimes;

    [Header("Object Reference")]
    [SerializeField] private Interactable interactable;
    [SerializeField] private BossSceneController bossSceneController;

    [Header("Dynamic Data")]
    [SerializeField] private static int currentActionTimes;
    [SerializeField] private bool isActived = false;


    private void Start()
    {
        currentActionTimes = 0;
    }

    private void Update()
    {
        if (currentActionTimes >= neededActionTimes)
        {
            Destroy(gameObject);
        }
    }

    public void OnInteraction()
    {
        if (isActived) return;

        currentActionTimes++;
        isActived = true;
        interactable.enabled = false;

        if (currentActionTimes >= neededActionTimes)
        {
            bossSceneController.SummonBoss();
        }
    }
}
