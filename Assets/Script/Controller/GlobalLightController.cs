using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Gradient gradient;
    [SerializeField] private float dayTime, nightTime;
    [SerializeField] private float transformTimeGap;

    [Header("Dynamic Data")]
    [SerializeField] private float timeElapse;
    [SerializeField] private GlobalTime time = GlobalTime.Day;

    [Header("Object Reference")]
    [SerializeField] private Light2D globalLight;

    enum GlobalTime
    {
        Day, Night
    }

    private void Start()
    {
        globalLight = GetComponent<Light2D>();
        globalLight.color = gradient.Evaluate(1);
    }

    private void Update()
    {
        timeElapse += Time.deltaTime;

        if(timeElapse >= dayTime && time == GlobalTime.Day)
        {
            StartCoroutine(DayNightTransform(time));
            time = GlobalTime.Night;
        }
        else if(timeElapse >= nightTime && time == GlobalTime.Night)
        {
            StartCoroutine(DayNightTransform(time));
            time = GlobalTime.Day;
        }
    }

    private IEnumerator DayNightTransform(GlobalTime globalTime)
    {
        switch (globalTime)
        {
            case GlobalTime.Day:
                for (float i = 1f; i > 0; i -= 0.01f)
                {
                    globalLight.color = gradient.Evaluate(i);
                    timeElapse = 0;
                    yield return new WaitForSeconds(transformTimeGap);
                }
                break;
            case GlobalTime.Night:
                for (float i = 0f; i < 1; i += 0.01f)
                {
                    globalLight.color = gradient.Evaluate(i);
                    timeElapse = 0;
                    yield return new WaitForSeconds(transformTimeGap);
                }
                break;
        }
    }
}
