using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
    }

    public void saveData()
    {
        int currentSceneName = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("SavedLevel", currentSceneName);
        PlayerPrefs.Save();
    }

    public void ContinueButtonClicked()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ExitButtonClicked()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}
