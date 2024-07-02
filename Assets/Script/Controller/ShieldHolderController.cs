using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ShieldHolderController : MonoBehaviour
{
    [Header("Dynamic Data")]
    public static int currentCoulumnAmount;

    public Action shieldBreak;

    public static void Reset()
    {
        currentCoulumnAmount = 6;
    }

    private void Start()
    {
        Reset();
    }

    private void OnDestroy()
    {
        currentCoulumnAmount--;

        if (currentCoulumnAmount <= 0)
        {
            shieldBreak.Invoke();
        }
    }
}
