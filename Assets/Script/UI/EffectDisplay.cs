using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    [Header("Dynamic Data")]
    [SerializeField] private List<GameObject> effectionDisplayList = new();

    [Header("Object Reference")]
    [SerializeField] private GameObject effectDisplayModel;
    [SerializeField] private PlayerBehaviour player;


    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (effectionDisplayList.Count < player.GetEffectionList.Count)
        {
            //create display
            GameObject Item = Instantiate(
                effectDisplayModel,
                transform.position,
                Quaternion.identity,
                transform);
            effectionDisplayList.Add(Item);
        }
        else if (effectionDisplayList.Count > player.GetEffectionList.Count)
        {
            //delet display
            var Item = effectionDisplayList[effectionDisplayList.Count - 1];
            effectionDisplayList.Remove(Item);
            Destroy(Item);
        }

        foreach (PlayerBehaviour.Effection effectionItem in player.GetEffectionList)
        {
            int indexOfEffectionList = player.GetEffectionList.IndexOf(effectionItem);

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
