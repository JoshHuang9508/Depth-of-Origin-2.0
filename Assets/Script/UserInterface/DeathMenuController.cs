using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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

    public async void RespawnBtnClicked()
    {
        sceneLoader_town.Load();
        PlayerPrefs.SetInt("loadscene", 4);

        await Task.Delay(2000);
        player.RevivePlayer();
    }

    public async void TitleBtnClicked()
    {
        sceneLoader_title.Load();
    }
}
