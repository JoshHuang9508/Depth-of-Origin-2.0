using UnityEngine;
using System.Threading.Tasks;

public class DeathMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PlayerBehaviour player;


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public async void Respawn()
    {
        sceneLoader.SetSceneLoaderContent(4);
        sceneLoader.Load();
        PlayerPrefs.SetInt("loadscene", 4);

        await Task.Delay(2000);
        player.RevivePlayer();

        gameObject.SetActive(false);
    }

    public void BackToTitle()
    {
        sceneLoader.SetSceneLoaderContent(0);
        sceneLoader.Load();

        gameObject.SetActive(false);
    }
}
