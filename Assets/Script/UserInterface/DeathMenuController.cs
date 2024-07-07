using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SceneLoader sceneLoader_town;
    [SerializeField] private SceneLoader sceneLoader_title;

    private PlayerBehaviour player;

    private void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by DeathMenuController.cs)");
        }
    }

    public void RespawnBtnClicked()
    {
        player.RevivePlayer();
        sceneLoader_town.Load();

        PlayerPrefs.SetInt("loadscene", 4);
    }

    public void TitleBtnClicked()
    {
        sceneLoader_title.Load();

        SceneManager.LoadScene("Loading");
    }
}
