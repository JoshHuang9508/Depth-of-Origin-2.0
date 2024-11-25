using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [Header("Attributes")]
    public LoadType loadType = LoadType.Scene;

    [Header("Attributes(Chunk)")]
    public GameObject transformPos;

    [Header("Attributes(Scene)")]
    public int sceneNum = 0;

    //Runtime data
    private PlayerBehaviour player;
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

        player.camEffect.PlayCamEffect(CamEffect.CamEffectType.CrossfadeIn);
        inAction = true;
        await Task.Delay(1500);

        switch (loadType)
        {
            case LoadType.Scene:
                await Load(SceneManager.LoadSceneAsync("Load", LoadSceneMode.Additive));
                await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                LoadingScene loadingScene = await GetLoadingScene();
                loadingScene.PlayLoadAnimation();
                await Task.Delay(100);
                await Load(SceneManager.LoadSceneAsync(sceneNum, LoadSceneMode.Additive));
                SceneControl sceneControl = await GetTargetSceneControl();
                await sceneControl.SceneSetup();
                await SceneManager.UnloadSceneAsync("Load");

                GameObject.FindWithTag("Player").transform.position = GameObject.FindWithTag("Respawn").transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = GameObject.FindWithTag("Respawn").transform.position + new Vector3(0, 0, -10);
                break;

            case LoadType.Chunk:
                GameObject.FindWithTag("Player").transform.position = transformPos.transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = transformPos.transform.position + new Vector3(0, 0, -10); ;
                break;
        }

        await Task.Delay(500);
        player.camEffect.PlayCamEffect(CamEffect.CamEffectType.CrossfadeOut);
        inAction = false;
    }

    private async Task Load(AsyncOperation progress)
    {
        while (!progress.isDone) await Task.Yield();
    }

    private async Task<LoadingScene> GetLoadingScene()
    {
        LoadingScene loadingScene = null;

        while (loadingScene == null)
        {
            try
            {
                loadingScene = GameObject.FindWithTag("Load").GetComponent<LoadingScene>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }

            await Task.Yield();
        }

        return loadingScene;
    }

    private async Task<SceneControl> GetTargetSceneControl()
    {
        SceneControl sceneControl = null;

        while (sceneControl == null)
        {
            try
            {
                GameObject[] gameObjects = null;
                gameObjects = GameObject.FindGameObjectsWithTag("SceneControl");

                foreach (GameObject gameObject in gameObjects)
                {
                    SceneControl sc = gameObject.GetComponent<SceneControl>();

                    if (sc != null && sc.sceneNum == sceneNum)
                    {
                        sceneControl = sc;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
            }

            await Task.Yield();
        }

        return sceneControl;
    }

    public void SetSceneLoaderContent(int sceneNum)
    {
        this.loadType = LoadType.Scene;
        this.sceneNum = sceneNum;
    }

    public void SetSceneLoaderContent(GameObject transformPos)
    {
        this.loadType = LoadType.Chunk;
        this.transformPos = transformPos;
    }
}
