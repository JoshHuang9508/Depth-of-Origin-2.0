using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchLightController : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private Light2D torchLight;
    [SerializeField] private Animator animator;

    [Header("Dynamic Data")]
    public bool extinguished = false;

    void Start()
    {
        torchLight = GetComponentInChildren<Light2D>();
        animator = GetComponent<Animator>();
        if(!extinguished) StartCoroutine(torchLightShining());
    }

    public void LightTorch()
    {
        extinguished = false;
        if (!extinguished) StartCoroutine(torchLightShining());
    }

    private void Update()
    {
        animator.SetBool("Extinguished", extinguished);
    }

    private IEnumerator torchLightShining()
    {
        float targetIntensityValue = Random.Range(0.6f, 0.8f);
        float currentIntensityValue = torchLight.intensity;
        int mutiplier = currentIntensityValue < targetIntensityValue ? 1 : -1;
        while (torchLight.intensity * mutiplier <= targetIntensityValue * mutiplier)
        {
            torchLight.intensity += 0.05f * mutiplier;
            yield return new WaitForSeconds(Random.Range(0.1f,0.05f));
        }
        StartCoroutine(torchLightShining());
    }
}
