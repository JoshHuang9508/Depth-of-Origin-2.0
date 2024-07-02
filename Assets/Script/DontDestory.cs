using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestoryOnLoad : MonoBehaviour
{
    private static DontDestoryOnLoad instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if(activeScene == scene)
        {
            GameObject.FindWithTag("Player").transform.position = GameObject.FindWithTag("Respawn").transform.position;
            GameObject.FindWithTag("CameraHold").transform.position = GameObject.FindWithTag("Respawn").transform.position;
            GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().OnSceneLoaded();
        }
    }
}
