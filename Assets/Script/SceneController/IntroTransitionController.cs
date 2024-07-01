using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTransitionController : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private SceneLoaderController sceneLoader;

    [SerializeField] string[] dialog = {
        "你走進了森林",
        "過了很長一段時間",
        "幾道刺眼的光透射進來",
        "你知道你已穿過森林",
        "隨之映入眼簾的",
        "是一座生氣盎然的村莊"
    };
    [SerializeField] private float fadeInDuration,fadeOutDuration;
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
        text.color = new Color(text.color.r,text.color.g,text.color.b,0f);
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

        if(index != dialog.Length - 1)
        {
            StartCoroutine(SetTextContent(index + 1));
        }
        else
        {
            PlayerPrefs.SetInt("loadscene", 4);
            sceneLoader.Load();
        }
    }
}
