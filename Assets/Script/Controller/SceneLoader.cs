using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [Header("Setting")]
    public LoadType loadType = LoadType.Scene;
    
    [Header("Setting(Chunk)")]
    public GameObject transformPos;

    [Header("Setting(Scene)")]
    public int SceneNum = 0;

    [Header("Reference")]
    public PlayerBehaviour player;

    //Runtime data
    public static bool inAction = false;

    public enum LoadType
    {
        Scene, Chunk
    }

    private void Update()
    {
        //Get player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by SceneLoader.cs)");
        }
    }

    public async void Load()
    {
        if (inAction) return;

        player.cutscene.SetTrigger("Start");
        inAction = true;
        await Task.Delay(1500);

        switch (loadType)
        {
            case LoadType.Scene:
                await SceneManager.LoadSceneAsync(SceneNum, LoadSceneMode.Additive);
                GameObject.FindWithTag("Player").transform.position = GameObject.FindWithTag("Respawn").transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = GameObject.FindWithTag("Respawn").transform.position + new Vector3(0, 0, -10);
                break;

            case LoadType.Chunk:
                GameObject.FindWithTag("Player").transform.position = transformPos.transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = transformPos.transform.position + new Vector3(0, 0, -10); ;
                break;
        }

        await Task.Delay(1500);
        player.cutscene.SetTrigger("End");
        inAction = false;
    }

    public void SetSceneLoaderContent(int sceneNum)
    {
        this.loadType = LoadType.Scene;
        this.SceneNum = sceneNum;
    }

    public void SetSceneLoaderContent(GameObject transformPos)
    {
        this.loadType = LoadType.Chunk;
        this.transformPos = transformPos;
    }
}
