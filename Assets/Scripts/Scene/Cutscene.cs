using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] string[] dialog = {
        "You walked into forest.",
        "And the night comes.",
        "You hear lots of monster yieling.",
        "The voice in your head still remain.",
        "Then you finally fall asleep.",
        "'...'",
        "'cOMe...'",
        "'... tHerE iS...... ThE eNd.'",
        "'... aFraId.'",
        "Your head is full of that annoying sound.",
        "Then, the morning.",
        "You wake up, but with really tired body.",
        "You have to mOve.",
        "YOu KnOw iT.",
        "YOU HAVE TO MOVE.",
        "TO YOUR END."
    };
    [SerializeField] private float fadeInDuration,fadeOutDuration;
    [SerializeField] private float showTipTime = 1f;

    [SerializeField] private PlayerBehaviour player;

    private void Start()
    {
        mainText.text = "";
        tipText.text = "";
        try { player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>(); } catch { }
        StartCoroutine(ShowMainText(0));
    }

    IEnumerator ShowMainText(int index)
    {
        player.SetActive(false);

        mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b,0f);
        mainText.text = dialog[index];
        tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0f);
        tipText.text = "-> Click to Continue <-";

        while (mainText.color.a < 1f)
        {
            float alpha = Mathf.Clamp01(mainText.color.a + (Time.deltaTime / fadeInDuration));
            mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(showTipTime);

        while (tipText.color.a < 1f)
        {
            float alpha = Mathf.Clamp01(tipText.color.a + (Time.deltaTime / fadeInDuration));
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, alpha);
            yield return null;
        }

        while (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            yield return null;
        }

        tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0f);

        while (mainText.color.a > 0f)
        {
            float alpha = Mathf.Clamp01(mainText.color.a - (Time.deltaTime / fadeOutDuration));
            mainText.color = new Color(mainText.color.r, mainText.color.g, mainText.color.b, alpha);
            yield return null;
        }

        if(index != dialog.Length - 1)
        {
            StartCoroutine(ShowMainText(index + 1));
        }
        else
        {
            //PlayerPrefs.SetInt("loadscene", 3);
            sceneLoader.Load();
            player.SetActive(true);
        }

        yield break;
    }
}
