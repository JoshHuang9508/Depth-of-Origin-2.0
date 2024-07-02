using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    public void RespawnBtnClicked()
    {
        player.GetComponent<PlayerBehaviour>().RevivePlayer();

        PlayerPrefs.SetInt("loadscene", 4);

        SceneManager.LoadScene("Loading");
    }

    public void TitleBtnClicked()
    {
        PlayerPrefs.SetInt("loadscene", 0);

        SceneManager.LoadScene("Loading");
    }
}
