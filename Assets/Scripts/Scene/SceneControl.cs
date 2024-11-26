using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SceneControl : MonoBehaviour
{
    [Header("Settings")]
    public int sceneNum;
    public List<Manager> managers;

    [Header("Status")]
    public bool isSceneSetup;

    public async Task<bool> SceneSetup()
    {
        while (!isSceneSetup)
        {
            isSceneSetup = true;
            foreach(Manager manager in managers)
            {
                isSceneSetup = manager.isSetup && isSceneSetup;
            }
            await Task.Yield();
        }
        return true;
    }
}

public class Manager : MonoBehaviour
{
    [Header("Manager status")]
    public bool isSetup;
}
