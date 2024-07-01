using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderController : MonoBehaviour
{
    [Header("Setting")]
    public LoadType loadType = LoadType.Scene;
    public int SceneNum = 0;
    public GameObject transformPos;

    [Header("Dynamic Data")]
    public static bool inAction = false;

    [Header("Object Reference")]
    public Animator transition;

    public enum LoadType
    {
        Scene, Chunk
    }


    private void Start()
    {
        transition = GameObject.FindWithTag("Transition").GetComponent<Animator>();
    }

    public void Load()
    {
        if (inAction) return;
        StartCoroutine(Load_delay());
    }

    private IEnumerator Load_delay()
    {
        transition.SetTrigger("Start");
        inAction = true;

        yield return new WaitForSeconds(1.5f);

        switch (loadType)
        {
            case LoadType.Scene:
                SceneManager.LoadScene(SceneNum, LoadSceneMode.Single);
                GameObject.FindWithTag("Player").transform.position = GameObject.FindWithTag("Respawn").transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = GameObject.FindWithTag("Respawn").transform.position + new Vector3(0, 0, -10);
                transition.SetTrigger("End");

                inAction = false;

                break;

            case LoadType.Chunk:
                GameObject.FindWithTag("Player").transform.position = transformPos.transform.position;
                GameObject.FindWithTag("CameraHold").transform.position = transformPos.transform.position + new Vector3(0, 0, -10); ;
                transition.SetTrigger("End");

                yield return new WaitForSeconds(0.5f);
                inAction = false;

                break;
        }
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
