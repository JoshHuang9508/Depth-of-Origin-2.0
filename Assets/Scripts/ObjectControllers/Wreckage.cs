using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wreckage : MonoBehaviour
{
    private float timeElapse = 0;

    void Update()
    {
        timeElapse += Time.deltaTime;
        if(timeElapse >= 20)
        {
            Destroy(gameObject);
        }
    }
}
