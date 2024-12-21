using UnityEngine;
using System.Threading.Tasks;

public class DeathMenu : MonoBehaviour, IInterface
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PlayerBehaviour player;

    public bool IsActive { get; set; }

    public void Toggle()
    {
        if (IsActive) Close();
        else Open();
    }

    public void Open()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public async void Respawn()
    {
        sceneLoader.SetSceneLoaderContent(4);
        sceneLoader.Load();
        //PlayerPrefs.SetInt("loadscene", 4);

        await Task.Delay(2000);
        player.Revive();

        IsActive = false;
        gameObject.SetActive(false);
    }

    public void BackToTitle()
    {
        sceneLoader.SetSceneLoaderContent(0);
        sceneLoader.Load();

        IsActive = false;
        gameObject.SetActive(false);
    }
}
