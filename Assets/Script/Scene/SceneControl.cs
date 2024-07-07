using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SceneControl : MonoBehaviour
{
    public int sceneNum;
    public bool isSceneSetup;

    public async Task<bool> SceneSetup()
    {
        while (!isSceneSetup)
        {
            await Task.Yield();
        }
        return true;
    }
}
