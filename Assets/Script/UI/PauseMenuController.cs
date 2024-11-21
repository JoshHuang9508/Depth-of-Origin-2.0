using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Status")]
    public bool isActive = false;

    [Header("Reference")]
    [SerializeField] private SceneLoader sceneLoader_town;

    public void SaveGame()
    {
        // Deal with save game
    }

    public void Exit()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void Open()
    {
        isActive = true;
        gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Close()
    {
        isActive = false;
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
