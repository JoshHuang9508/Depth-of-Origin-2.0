using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckageController : MonoBehaviour
{
    [Header("Dynamic Data")]
    [SerializeField] float timeElapse = 0;

    void Update()
    {
        timeElapse += Time.deltaTime;
        if(timeElapse >= 20)
        {
            Destroy(gameObject);
        }
    }
}
