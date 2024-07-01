using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameController : MonoBehaviour
{
    public int sceneIndex;
    public ProgressBar progressBar;
    void Start()
    {
        progressBar.currentPercent = 0f;
        sceneIndex = PlayerPrefs.GetInt("loadscene");
        StartCoroutine(loading());
    }

    IEnumerator loading()
    {
        yield return null;        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.currentPercent = progress * 100f;
            yield return null;
        }
    }
}
