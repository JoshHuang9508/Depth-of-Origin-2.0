using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        // Deal with save game
    }

    public void Exit()
    {
        Time.timeScale = 1.0f;

        sceneLoader.SetSceneLoaderContent(0);
        sceneLoader.Load();

        gameObject.SetActive(false);
    }

    public void Open()
    {
        Time.timeScale = 0.0f;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        Time.timeScale = 1.0f;

        gameObject.SetActive(false);
    }
}
