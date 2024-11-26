using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] TextMeshProUGUI text;

    public async void PlayLoadAnimation()
    {
        await Task.Delay(300);
        text.text = "Now Loading.";
        await Task.Delay(300);
        text.text = "Now Loading..";
        await Task.Delay(300);
        text.text = "Now Loading...";
        await Task.Delay(300);
        text.text = "Now Loading....";
        await Task.Delay(300);
        text.text = "Now Loading.....";
        await Task.Delay(300);
        text.text = "Now Loading......";
        await Task.Delay(300);
        PlayLoadAnimation();
    }
}
