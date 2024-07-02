using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject effectDisplayModel;

    //Runtime data
    private List<GameObject> effectionDisplayList = new();
    private PlayerBehaviour player;


    private void Update()
    {
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by EffectionDisplay.cs)");
        }

        if (player != null)
        {
            SetDisplay();
        }
            
    }

    private void SetDisplay()
    {
        if (effectionDisplayList.Count < player.effectionList.Count)
        {
            //create display
            GameObject Item = Instantiate(
                effectDisplayModel,
                transform.position,
                Quaternion.identity,
                transform);
            effectionDisplayList.Add(Item);
        }
        else if (effectionDisplayList.Count > player.effectionList.Count)
        {
            //delet display
            var Item = effectionDisplayList[effectionDisplayList.Count - 1];
            effectionDisplayList.Remove(Item);
            Destroy(Item);
        }

        foreach (Effection effectionItem in player.effectionList)
        {
            int indexOfEffectionList = player.effectionList.IndexOf(effectionItem);

            foreach (GameObject effectionDisplayerItem in effectionDisplayList)
            {
                int indexOfEffectionDisplayList = effectionDisplayList.IndexOf(effectionDisplayerItem);

                if (indexOfEffectionDisplayList == indexOfEffectionList)
                {
                    //update display
                    TextMeshProUGUI text = effectionDisplayerItem.GetComponentInChildren<TextMeshProUGUI>();
                    Image image = effectionDisplayerItem.GetComponentInChildren<Image>();

                    image.sprite = effectionItem.effectingItem.Image;

                    int min = Mathf.FloorToInt(effectionItem.effectingTime / 60);
                    int sec = Mathf.FloorToInt(effectionItem.effectingTime % 60);
                    text.text = (min < 10 ? "0" + min : min) + ":" + (sec < 10 ? "0" + sec : sec);
                }
            }
        }
    }
}
