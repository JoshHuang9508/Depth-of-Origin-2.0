using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour, IInterface
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;

    public bool IsActive { get; set; }

    public void Toggle()
    {
        if (IsActive) Close();
        else Open();
    }

    public void Open()
    {
        Time.timeScale = 0.0f;

        IsActive = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        Time.timeScale = 1.0f;

        IsActive = false;
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

        IsActive = false;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }
}
