using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroTransitionController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private SceneLoaderController sceneLoader;

    [SerializeField]
    string[] dialog = {
        "你走入黑暗",
        "意識模糊",
        "只聽見耳語說著:",
        "他們會被驅逐",
        "但他們不會消散",
        "這是無法避免的命運",
        "你無法拒絕",
        "啟程吧",
        "覺悟吧",
        "以探索",
        "起源之深"
    };
    [SerializeField] private float fadeInDuration, fadeOutDuration;
    [SerializeField] private float waitTime;

    private void Start()
    {
        text.text = "";
        text.gameObject.SetActive(true);
        waitTime = 0.5f;
        StartCoroutine(SetTextContent(0));
    }



    IEnumerator SetTextContent(int index)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        text.text = dialog[index];

        while (text.color.a < 1f)
        {
            float alpha = Mathf.Clamp01(text.color.a + (Time.deltaTime / fadeInDuration));
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (text.color.a > 0f)
        {
            float alpha = Mathf.Clamp01(text.color.a - (Time.deltaTime / fadeOutDuration));
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(waitTime);

        if (index != dialog.Length - 1)
        {
            StartCoroutine(SetTextContent(index + 1));
        }
        else
        {
            PlayerPrefs.SetInt("loadscene", 0);
            sceneLoader.Load();
        }
    }
}
